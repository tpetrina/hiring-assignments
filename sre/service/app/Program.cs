using System.Reflection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx,lc)=>lc
    .WriteTo.Console());
builder.Services.AddHealthChecks();
builder.Configuration.AddEnvironmentVariables(prefix: "APP_");
var app = builder.Build();

var serviceUri = app.Configuration["ServiceUri"];
var serviceProxy = new HttpClient();
app.Logger.LogInformation($"ServiceUri: {serviceUri}");

app.UseMetricServer();
app.UseHttpMetrics();
var failed_requests = Metrics.CreateCounter("failed_requests", "Total number of failed requests made");

app.MapGet("/version", () =>
{
    return Assembly
        .GetExecutingAssembly()
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
        ?.InformationalVersion ?? "none";
    ;
});
app.MapGet("/{id}", async (string id) =>
{
    Validators.CheckId(id);

    try
    {
        var message = await serviceProxy.GetAsync($"{serviceUri}{id}");
        return Results.Stream(await message.Content.ReadAsStreamAsync(), message.Content.Headers.ContentType?.ToString());
    }
    catch (Exception ex)
    {
        failed_requests.Inc();
        app.Logger.LogError(ex, "Unable to reach service.");
        return Results.StatusCode(503);
    }
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions()
{
    Predicate = (check) => check.Tags.Contains("ready"),
});
app.MapHealthChecks("/health/live", new HealthCheckOptions()
{
    Predicate = (_) => false
});

app.Run();

public static class Validators
{
    public static void CheckId(string id)
    {
        ArgumentNullException.ThrowIfNull(id);
        if (!int.TryParse(id, out var parsed) || parsed < 1)
        {
            throw new ArgumentException("Should be a number", nameof(id));
        }
    }
}