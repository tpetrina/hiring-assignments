using System;
using Xunit;

namespace tests;

public class ValidatorTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("1a")]
    [InlineData("a1")]
    [InlineData("-")]
    [InlineData("-1")]
    [InlineData("0")]
    [InlineData("1.1")]
    public void Validators_CheckId__ThrowsForInvalidInput(string input)
    {
        Assert.ThrowsAny<Exception>(() => Validators.CheckId(input));
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("54545")]
    public void Validators_CheckId__DoesntThrowForValidInput(string input)
    {
        Validators.CheckId(input);
    }
}