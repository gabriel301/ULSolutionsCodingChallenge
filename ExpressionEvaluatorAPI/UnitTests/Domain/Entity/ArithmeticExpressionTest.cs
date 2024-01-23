using Domain.Entities.Expression;
using Domain.Exceptions;
using Domain.Interfaces;
using FluentAssertions;
using NSubstitute;
using Shared.ReShared.Resources.Validationsoures.Validation;

namespace UnitTests.Domain.Entity;

[Collection(nameof(ArithmeticExpressionTestFixture))]
public class ArithmeticExpressionTest
{

    private readonly ArithmeticExpressionTestFixture _expressionFixture;

    public ArithmeticExpressionTest(ArithmeticExpressionTestFixture expressionFixture) 
    {
        _expressionFixture = expressionFixture;
    }
    
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "StringExpression")]
    public void Instantiate()
    {
        
        string data = "1+1+1";

        ArithmeticExpression expression = ArithmeticExpression.Create(data, _expressionFixture.GetExpressionTreeSubstitute());

        expression.Should().NotBeNull();
        expression.ExpresionString.Should().Be(data);
    }


    [Fact(DisplayName = nameof(ExpressionTreeNull))]
    [Trait("Domain", "StringExpression")]
    public void ExpressionTreeNull()
    {

        string data = "1+1+1";

        Action action = () => { ArithmeticExpression.Create(data, null); };

        action.Should().Throw<DomainException>().WithMessage(ValidationResources.Expression_Tree_Null);

        action.Should().Throw<DomainException>().Which.Expression.Should().Be(string.Empty);
    }

    [Theory(DisplayName = nameof(IsNullOrEmptyString))]
    [Trait("Domain", "StringExpression")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")] //Spaces
    [InlineData("       ")] //Tabs
    public void IsNullOrEmptyString(string? data)
    {
        Action action = () => { ArithmeticExpression.Create(data, _expressionFixture.GetExpressionTreeSubstitute()); };

        action.Should().Throw<DomainException>().WithMessage(ValidationResources.Null_Or_Empty_String);

        action.Should().Throw<DomainException>().Which.Expression.Should().Be(string.Empty);
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
        Action action = () => { ArithmeticExpression.Create(data, _expressionFixture.GetExpressionTreeSubstitute()); };

        var exception = Assert.Throws<DomainException>(action);

        action.Should().Throw<DomainException>().WithMessage(ValidationResources.Invalid_Characters);

        action.Should().Throw<DomainException>().Which.Expression.Should().Be(data);
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
        Action action = () => { ArithmeticExpression.Create(data, _expressionFixture.GetExpressionTreeSubstitute()); };

        var exception = Assert.Throws<DomainException>(action);


        action.Should().Throw<DomainException>().WithMessage(ValidationResources.Consecutive_Operators);
        action.Should().Throw<DomainException>().Which.Expression.Should().Be(data);
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
        Action action = () => { ArithmeticExpression.Create(data, _expressionFixture.GetExpressionTreeSubstitute()); };

        var exception = Assert.Throws<DomainException>(action);

        action.Should().Throw<DomainException>().WithMessage(ValidationResources.Starts_Or_Ends_With_Operator);
        action.Should().Throw<DomainException>().Which.Expression.Should().Be(data);

    }


    [Theory(DisplayName = nameof(ContainsOnlyDigits))]
    [Trait("Domain", "StringExpression")]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("123")]
    [InlineData("12345")]
    public void ContainsOnlyDigits(string? data)
    {
        Action action = () => { ArithmeticExpression.Create(data, _expressionFixture.GetExpressionTreeSubstitute()); };

        var exception = Assert.Throws<DomainException>(action);

        action.Should().Throw<DomainException>().WithMessage(ValidationResources.Contains_Only_Digits);
        action.Should().Throw<DomainException>().Which.Expression.Should().Be(data);

    }

}

