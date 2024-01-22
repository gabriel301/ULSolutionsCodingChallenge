using Domain.Entities.Expression;
using Domain.Exceptions;
using FluentAssertions;
using Shared.ReShared.Resources.Validationsoures.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Domain.Entity;
public class ExpressionTreeTest
{

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "ExpressionTree")]
    public void Instantiate()
    {

        string data = "1+1/2+10";

        ExpressionTree tree = ExpressionTree.Create(data);

        tree.Should().NotBeNull();
        tree.Expression.Should().Be(data);
    }


    //[Theory(DisplayName = nameof(IsNullOrEmptyString))]
    //[Trait("Domain", "ExpressionTree")]
    //[InlineData(null)]
    //[InlineData("")]
    //[InlineData("   ")] //Spaces
    //[InlineData("       ")] //Tabs
    //public void IsNullOrEmptyString(string? data)
    //{
    //    Action action = () => { ExpressionTree.Create(data); };

    //    action.Should().Throw<DomainException>().WithMessage(ValidationResources.ResourceManager.GetString("Null_Or_Empty_String"));

    //    action.Should().Throw<DomainException>().Which.Expression.Should().Be(string.Empty);
    //}


    //[Theory(DisplayName = nameof(ContainsInvalidCharacters))]
    //[Trait("Domain", "ExpressionTree")]
    //[InlineData("A+1+3")]
    //[InlineData("1+2=3")]
    //[InlineData("1.1+2+3")]
    //[InlineData("1 + 1")] //Spaces
    //[InlineData("1  +1")] //Tabs
    //[InlineData("1+1!2")] //Tabs
    //public void ContainsInvalidCharacters(string? data)
    //{
    //    Action action = () => { ExpressionTree.Create(data); };

    //    var exception = Assert.Throws<DomainException>(action);

    //    action.Should().Throw<DomainException>().WithMessage(ValidationResources.ResourceManager.GetString("Invalid_Characters"));

    //    action.Should().Throw<DomainException>().Which.Expression.Should().Be(data);
    //}


    //[Theory(DisplayName = nameof(ContainsSequentialOperators))]
    //[Trait("Domain", "ExpressionTree")]
    //[InlineData("++1+2")]
    //[InlineData("--1+2")]
    //[InlineData("//1+1")]
    //[InlineData("**1+2")]
    //[InlineData("1+-2")]
    //[InlineData("1-*2")]
    //[InlineData("1-*+/-2")]
    //[InlineData("1-2+-")]
    //public void ContainsSequentialOperators(string? data)
    //{
    //    Action action = () => { ExpressionTree.Create(data); };

    //    var exception = Assert.Throws<DomainException>(action);


    //    action.Should().Throw<DomainException>().WithMessage(ValidationResources.ResourceManager.GetString("Consecutive_Operators"));
    //    action.Should().Throw<DomainException>().Which.Expression.Should().Be(data);
    //}


    //[Theory(DisplayName = nameof(StartsOrEndsWithOperators))]
    //[Trait("Domain", "ExpressionTree")]
    //[InlineData("+1+2")]
    //[InlineData("-1+2-")]
    //[InlineData("/1+1")]
    //[InlineData("*1+2")]
    //[InlineData("1+2-")]
    //[InlineData("1+2*")]
    //[InlineData("1-2/")]
    //[InlineData("+1-2-")]
    //public void StartsOrEndsWithOperators(string? data)
    //{
    //    Action action = () => { ExpressionTree.Create(data); };

    //    var exception = Assert.Throws<DomainException>(action);

    //    action.Should().Throw<DomainException>().WithMessage(ValidationResources.ResourceManager.GetString("Starts_Or_Ends_With_Operator"));
    //    action.Should().Throw<DomainException>().Which.Expression.Should().Be(data);

    //}


    //[Theory(DisplayName = nameof(ContainsOnlyDigits))]
    //[Trait("Domain", "ExpressionTree")]
    //[InlineData("0")]
    //[InlineData("1")]
    //[InlineData("123")]
    //[InlineData("12345")]
    //public void ContainsOnlyDigits(string? data)
    //{
    //    Action action = () => { ExpressionTree.Create(data); };

    //    var exception = Assert.Throws<DomainException>(action);

    //    action.Should().Throw<DomainException>().WithMessage(ValidationResources.ResourceManager.GetString("Contains_Only_Digits"));
    //    action.Should().Throw<DomainException>().Which.Expression.Should().Be(data);

    //}
}
