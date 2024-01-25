using FluentAssertions;
using NSubstitute;
using UL.Domain.Entities.ArithmeticExpression;
using UL.Domain.Events.ArithmeticExpression.Created;
using UL.Domain.Events.ArithmeticExpression.Evaluated;
using UL.Domain.Exceptions;
using UL.Domain.Exceptions.ArithmeticExpression;
using UL.Domain.Resources;
using UL.Domain.Services.Abstraction;
using UL.Shared.Resources;

namespace UL.Tests.Domain.Domain.Entity;
public class ArithmeticExpressionTest
{

    #region Setup
    private readonly IOperationService _operationServiceMock;
    public ArithmeticExpressionTest()
    {
        _operationServiceMock = Substitute.For<IOperationService>();

        //1+1+1 Test
        _operationServiceMock.Calculate("+", 1.0, 1.0).Returns(2.0);
        _operationServiceMock.Calculate("+", 2.0, 1.0).Returns(3.0);

        //4+5*2 test
        _operationServiceMock.Calculate("*", 5.0, 2.0).Returns(10.0);
        _operationServiceMock.Calculate("+", 4.0, 10.0).Returns(14.0);

        //4+5/2 test
        _operationServiceMock.Calculate("/", 5.0, 2.0).Returns(2.5);
        _operationServiceMock.Calculate("+", 4.0, 2.5).Returns(6.5);

        //4+5/2-1 test
        _operationServiceMock.Calculate("/", 5.0, 2.0).Returns(2.5);
        _operationServiceMock.Calculate("+", 4.0, 2.5).Returns(6.5);
        _operationServiceMock.Calculate("-", 6.5, 1.0).Returns(5.5);

        //1/0 test
        _operationServiceMock.Calculate("/", 1.0, 0.0).Returns(double.PositiveInfinity);

        //Double Max test
        _operationServiceMock.Calculate("+", Double.Parse(double.MaxValue.ToString("0.#")), Double.Parse(double.MaxValue.ToString("0.#"))).Returns(double.PositiveInfinity);
    }

    #endregion

    #region Theory Data
    public static TheoryData<string> InfinityValues =>
        new TheoryData<string>
        {
            "1/0",
            $"{double.MaxValue.ToString("0.#")}+{double.MaxValue.ToString("0.#")}"

        };

    #endregion

    #region Tests

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "ArithmeticExpression")]
    public void Instantiate()
    {

        string data = "1+1";

        ArithmeticExpressionEvaluator tree = ArithmeticExpressionEvaluator.Create(data);

        tree.Should().NotBeNull();
        tree.Expression.Should().Be(data);
        tree.GetDomainEvents().Should().HaveCount(1);
        tree.GetDomainEvents().Select(domainEvent => domainEvent).First().Should().BeOfType<ArithmeticExpressionCreatedEvent>();

    }


    [Theory(DisplayName = nameof(IsNullOrEmptyString))]
    [Trait("Domain", "ArithmeticExpression")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")] //Spaces
    [InlineData("       ")] //Tabs
    public void IsNullOrEmptyString(string? data)
    {
        Action action = () => { ArithmeticExpressionEvaluator.Create(data); };

        action.Should().Throw<DomainValidationException>().WithMessage(ValidationResources.Null_Or_Empty_String);

        action.Should().Throw<DomainValidationException>().Which.Expression.Should().Be(string.Empty);
    }


    [Theory(DisplayName = nameof(ContainsInvalidCharacters))]
    [Trait("Domain", "ArithmeticExpression")]
    [InlineData("A+1+3")]
    [InlineData("1+2=3")]
    [InlineData("1.1+2+3")]
    [InlineData("1 + 1")] //Spaces
    [InlineData("1  +1")] //Tabs
    [InlineData("1+1!2")]
    public void ContainsInvalidCharacters(string? data)
    {
        Action action = () => { ArithmeticExpressionEvaluator.Create(data); };

        var exception = Assert.Throws<DomainValidationException>(action);

        action.Should().Throw<DomainValidationException>().WithMessage(ValidationResources.Invalid_Characters);

        action.Should().Throw<DomainValidationException>().Which.Expression.Should().Be(data);
    }


    [Theory(DisplayName = nameof(ContainsSequentialOperators))]
    [Trait("Domain", "ArithmeticExpression")]
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
        Action action = () => { ArithmeticExpressionEvaluator.Create(data); };

        var exception = Assert.Throws<DomainValidationException>(action);


        action.Should().Throw<DomainValidationException>().WithMessage(ValidationResources.Consecutive_Operators);
        action.Should().Throw<DomainValidationException>().Which.Expression.Should().Be(data);
    }


    [Theory(DisplayName = nameof(StartsOrEndsWithOperators))]
    [Trait("Domain", "ArithmeticExpression")]
    [InlineData("+1+2")]
    [InlineData("-1+2-")]
    [InlineData("/1+1")]
    [InlineData("*1+2")]
    [InlineData("1+2-")]
    [InlineData("1+2*")]
    [InlineData("1-2/")]
    [InlineData("+1-2-")]
    [InlineData("-")]
    [InlineData("+")]
    [InlineData("/")]
    [InlineData("*")]
    public void StartsOrEndsWithOperators(string? data)
    {
        Action action = () => { ArithmeticExpressionEvaluator.Create(data); };

        var exception = Assert.Throws<DomainValidationException>(action);

        action.Should().Throw<DomainValidationException>().WithMessage(ValidationResources.Starts_Or_Ends_With_Operator);
        action.Should().Throw<DomainValidationException>().Which.Expression.Should().Be(data);

    }


    [Theory(DisplayName = nameof(ContainsOnlyDigits))]
    [Trait("Domain", "ArithmeticExpression")]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("123")]
    [InlineData("12345")]
    public void ContainsOnlyDigits(string? data)
    {
        Action action = () => { ArithmeticExpressionEvaluator.Create(data); };

        var exception = Assert.Throws<DomainValidationException>(action);

        action.Should().Throw<DomainValidationException>().WithMessage(ValidationResources.Contains_Only_Digits);
        action.Should().Throw<DomainValidationException>().Which.Expression.Should().Be(data);

    }


    [Theory(DisplayName = nameof(EvaluateExpression))]
    [Trait("Domain", "ArithmeticExpression")]
    [InlineData("4+5*2", 14.0)]
    [InlineData("4+5/2", 6.5)]
    [InlineData("4+5/2-1", 5.5)]
    [InlineData("1+1+1", 3.0)]
    public void EvaluateExpression(string? data, double expectedResult)
    {
        ArithmeticExpressionEvaluator tree = ArithmeticExpressionEvaluator.Create(data);

        double result = tree.Evaluate(_operationServiceMock);
        tree.GetDomainEvents().Should().HaveCount(2);
        tree.GetDomainEvents().Select(domainEvent => domainEvent).First().Should().BeOfType<ArithmeticExpressionCreatedEvent>();
        tree.GetDomainEvents().Select(domainEvent => domainEvent).Last().Should().BeOfType<ArithmeticExpressionEvaluatedEvent>();

        result.Should().Be(expectedResult);

    }


    [Theory(DisplayName = nameof(EvaluateExpressionInfinity))]
    [Trait("Domain", "ArithmeticExpression")]
    [MemberData(nameof(InfinityValues))]
    public void EvaluateExpressionInfinity(string data)
    {

        var tree = ArithmeticExpressionEvaluator.Create(data);
        Action action = () => { tree.Evaluate(_operationServiceMock); };

        var exception = Assert.Throws<ArithmeticExpressionEvaluationException>(action);

        action.Should().Throw<ArithmeticExpressionEvaluationException>().WithMessage(ArithmeticExpressionEvaluationExceptionResource.Infinity_Value);
        action.Should().Throw<ArithmeticExpressionEvaluationException>().Which.Expression.Should().Be(data);
    }

    #endregion
}
