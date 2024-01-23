using Domain.Entities.Expression;
using Domain.Events.ExpressionTree.Created;
using Domain.Exceptions;
using Domain.Exceptions.ExpressionTree;
using Domain.Resources;
using FluentAssertions;
using Shared.ReShared.Resources.Validationsoures.Validation;

namespace UnitTests.Domain.Entity;
public class ExpressionTreeTest
{

    public static TheoryData<string> InfinityValues =>
        new TheoryData<string>
        {
            "1/0",
            $"{double.MaxValue.ToString("0.#")}+{double.MaxValue.ToString("0.#")}"
            
        };

    
    
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "ExpressionTree")]
    public void Instantiate()
    {

        string data = "1+1";

        ExpressionTree tree = ExpressionTree.Create(data);

        tree.Should().NotBeNull();
        tree.GetNodeCount().Should().Be(data.Length);
        tree.Expression.Should().Be(data);
        tree.GetDomainEvents().Should().HaveCount(1);
        tree.GetDomainEvents().Select(domainEvent => domainEvent).First().Should().BeOfType<ExpressionTreeCreatedEvent>();
        tree.Dispose();
    }


    [Theory(DisplayName = nameof(IsNullOrEmptyString))]
    [Trait("Domain", "ExpressionTree")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")] //Spaces
    [InlineData("       ")] //Tabs
    public void IsNullOrEmptyString(string? data)
    {
        Action action = () => { ExpressionTree.Create(data); };

        action.Should().Throw<DomainValidationException>().WithMessage(ValidationResources.Null_Or_Empty_String);

        action.Should().Throw<DomainValidationException>().Which.Expression.Should().Be(string.Empty);
    }


    [Theory(DisplayName = nameof(ContainsInvalidCharacters))]
    [Trait("Domain", "ExpressionTree")]
    [InlineData("A+1+3")]
    [InlineData("1+2=3")]
    [InlineData("1.1+2+3")]
    [InlineData("1 + 1")] //Spaces
    [InlineData("1  +1")] //Tabs
    [InlineData("1+1!2")]
    public void ContainsInvalidCharacters(string? data)
    {
        Action action = () => { ExpressionTree.Create(data); };

        var exception = Assert.Throws<DomainValidationException>(action);

        action.Should().Throw<DomainValidationException>().WithMessage(ValidationResources.Invalid_Characters);

        action.Should().Throw<DomainValidationException>().Which.Expression.Should().Be(data);
    }


    [Theory(DisplayName = nameof(ContainsSequentialOperators))]
    [Trait("Domain", "ExpressionTree")]
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
        Action action = () => { ExpressionTree.Create(data); };

        var exception = Assert.Throws<DomainValidationException>(action);


        action.Should().Throw<DomainValidationException>().WithMessage(ValidationResources.Consecutive_Operators);
        action.Should().Throw<DomainValidationException>().Which.Expression.Should().Be(data);
    }


    [Theory(DisplayName = nameof(StartsOrEndsWithOperators))]
    [Trait("Domain", "ExpressionTree")]
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
        Action action = () => { ExpressionTree.Create(data); };

        var exception = Assert.Throws<DomainValidationException>(action);

        action.Should().Throw<DomainValidationException>().WithMessage(ValidationResources.Starts_Or_Ends_With_Operator);
        action.Should().Throw<DomainValidationException>().Which.Expression.Should().Be(data);

    }


    [Theory(DisplayName = nameof(ContainsOnlyDigits))]
    [Trait("Domain", "ExpressionTree")]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("123")]
    [InlineData("12345")]
    public void ContainsOnlyDigits(string? data)
    {
        Action action = () => { ExpressionTree.Create(data); };

        var exception = Assert.Throws<DomainValidationException>(action);

        action.Should().Throw<DomainValidationException>().WithMessage(ValidationResources.Contains_Only_Digits);
        action.Should().Throw<DomainValidationException>().Which.Expression.Should().Be(data);

    }


    [Theory(DisplayName = nameof(EvaluateExpression))]
    [Trait("Domain", "ExpressionTree")]
    [InlineData("4+5*2", 14.0)]
    [InlineData("4+5/2", 6.5)]
    [InlineData("4+5/2-1", 5.5)]
    [InlineData("1+1+1", 3.0)]
    [InlineData("1+2/3+4*5", 21.6666666)]
    [InlineData("1/2+3*4", 12.5)]
    [InlineData("1/2+3+4*5", 23.5)]
    [InlineData("1/2/6/34554+243-908/3453-1344*465767+134565/23454*1245-13243-344565+566754/54365*2342-324344*4543+45345-56564", -2099822865.897338533165544)]
    [InlineData("1231/6546+657657-442*7867/234+65767-242+657-2342*675/342", 704356.93074383)]

    public void EvaluateExpression(string? data, double expectedResult)
    {
        ExpressionTree tree = ExpressionTree.Create(data);

        double result = tree.Evaluate();
        tree.Dispose();

        result.Should().BeApproximately(expectedResult, 0.0001);

    }


    [Theory(DisplayName = nameof(EvaluateExpressionInfinity))]
    [Trait("Domain", "ExpressionTree")]
    [MemberData(nameof(InfinityValues))]
    public void EvaluateExpressionInfinity(string data)
    {

        var tree = ExpressionTree.Create(data);
        Action action = () => { tree.Evaluate(); };

        var exception = Assert.Throws<ExpressionTreeEvaluationException>(action);

        action.Should().Throw<ExpressionTreeEvaluationException>().WithMessage(ExpressionTreeEvaluationExpectionResource.Infinity_Value);
        action.Should().Throw<ExpressionTreeEvaluationException>().Which.Expression.Should().Be(data);
    }
}
