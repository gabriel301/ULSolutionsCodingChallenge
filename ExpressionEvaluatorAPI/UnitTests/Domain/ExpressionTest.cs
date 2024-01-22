using Domain.Entities;
using Domain.Exceptions;
using Shared.ReShared.Resources.Validationsoures.Validation;

namespace UnitTests.Domain;

public class ExpressionTest
{

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "StringExpression")]
    public void Instantiate()
    {
        string data = "1+1+1";

        StringExpression expression = new StringExpression(data);

        Assert.NotNull(expression);
        Assert.Equal(data, expression.ExpresionString);
    }


    [Theory(DisplayName = nameof(IsNullOrEmptyString))]
    [Trait("Domain", "StringExpression")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")] //Spaces
    [InlineData("       ")] //Tabs
    public void IsNullOrEmptyString(string? data)
    {
        Action action = () => { new StringExpression(data); };

        var exception = Assert.Throws<DomainException>(action);

        Assert.NotNull(exception);
        Assert.IsType(typeof(DomainException), exception);
        Assert.Equal(ValidationResources.ResourceManager.GetString("Null_Or_Empty_String"), exception.Message);

    }


    [Theory(DisplayName = nameof(ContainsInvalidCharacters))]
    [Trait("Domain", "StringExpression")]
    [InlineData("A+1+3")]
    [InlineData("1+2=3")]
    [InlineData("1.1+2+3")]
    [InlineData("1 + 1")] //Spaces
    [InlineData("1  +1")] //Tabs
    [InlineData("1+1!2")] //Tabs
    public void ContainsInvalidCharacters(string? data)
    {
        Action action = () => { new StringExpression(data); };

        var exception = Assert.Throws<DomainException>(action);

        Assert.NotNull(exception);
        Assert.IsType(typeof(DomainException), exception);
        Assert.Equal(ValidationResources.ResourceManager.GetString("Invalid_Characters"), exception.Message);

    }


    [Theory(DisplayName = nameof(ContainsSequentialOperators))]
    [Trait("Domain", "StringExpression")]
    [InlineData("++1+2")]
    [InlineData("--1+2")]
    [InlineData("//1+1")]
    [InlineData("**1+2")]
    [InlineData("1+-2")]
    [InlineData("1-*2")]
    [InlineData("1-*+/-2")]
    [InlineData("1-2+-")]
    public void ContainsSequentialOperators(string? data)
    {
        Action action = () => { new StringExpression(data); };

        var exception = Assert.Throws<DomainException>(action);

        Assert.NotNull(exception);
        Assert.IsType(typeof(DomainException), exception);
        Assert.Equal(ValidationResources.ResourceManager.GetString("Consecutive_Operators"), exception.Message);

    }


    [Theory(DisplayName = nameof(StartsOrEndsWithOperators))]
    [Trait("Domain", "StringExpression")]
    [InlineData("+1+2")]
    [InlineData("-1+2-")]
    [InlineData("/1+1")]
    [InlineData("*1+2")]
    [InlineData("1+2-")]
    [InlineData("1+2*")]
    [InlineData("1-2/")]
    [InlineData("+1-2-")]
    public void StartsOrEndsWithOperators(string? data)
    {
        Action action = () => { new StringExpression(data); };

        var exception = Assert.Throws<DomainException>(action);

        Assert.NotNull(exception);
        Assert.IsType(typeof(DomainException), exception);
        Assert.Equal(ValidationResources.ResourceManager.GetString("Starts_Or_Ends_With_Operator"), exception.Message);

    }


    [Theory(DisplayName = nameof(ContainsOnlyDigits))]
    [Trait("Domain", "StringExpression")]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("123")]
    [InlineData("12345")]
    public void ContainsOnlyDigits(string? data)
    {
        Action action = () => { new StringExpression(data); };

        var exception = Assert.Throws<DomainException>(action);

        Assert.NotNull(exception);
        Assert.IsType(typeof(DomainException), exception);
        Assert.Equal(ValidationResources.ResourceManager.GetString("Contains_Only_Digits"), exception.Message);

    }


}

