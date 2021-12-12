# --- BASE  ---
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
ARG APP_VERSION=0.0.1
WORKDIR /app

COPY app/app.csproj ./app/app.csproj
COPY tests/tests.csproj ./tests/tests.csproj
COPY sha.sln ./
RUN dotnet restore

COPY ./app ./app
WORKDIR /app/app
RUN dotnet publish -c Release -o out --version-suffix=$APP_VERSION

# --- TEST  ---
FROM build-env as test
WORKDIR /app
COPY ./tests ./tests
RUN dotnet test tests/tests.csproj

# --- BUILD ---
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/app/out .
ENTRYPOINT ["dotnet", "app.dll"]
