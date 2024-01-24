using MediatR;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UL.Application.Expression.Command;
using UL.Application.Expression.Handlers;
using UL.Domain.Events.ExpressionTree.Created;
using UL.Domain.Events.ExpressionTree.Evaluated;
using UL.Domain.Exceptions;
using UL.Shared.Events;
using static System.Collections.Specialized.BitVector32;
using UL.Shared.Resources;
using static System.Runtime.InteropServices.JavaScript.JSType;
using UL.Entities.Expression;
using FluentAssertions;
using FluentAssertions.Common;
using UL.Domain.Resources;
using UL.Domain.Exceptions.ExpressionTree;

namespace UL.Tests.Application.Application.Handler;
public class EvaluateExpressionCommandHandlerTest
{
    private readonly EvaluateExpressionCommandHandler _handler;
    private readonly IPublisher _publisherMock;

    #region Setup
    public EvaluateExpressionCommandHandlerTest() 
    {
        _publisherMock = Substitute.For<IPublisher>();
        _publisherMock.Publish(Arg.Any<ExpressionTreeCreatedEvent>(), Arg.Any<CancellationToken>());
        _publisherMock.Publish(Arg.Any<ExpressionTreeEvaluatedEvent>(), Arg.Any<CancellationToken>());
        _handler = new EvaluateExpressionCommandHandler(_publisherMock);
    }

    #endregion

    #region Theory Data
    public static TheoryData<EvaluateExpressionCommand> NullOrEmptyValues =>
     new TheoryData<EvaluateExpressionCommand>
     {
            new EvaluateExpressionCommand(null),
            new EvaluateExpressionCommand(""),
            new EvaluateExpressionCommand("  "),
            new EvaluateExpressionCommand("     ")

     };


    public static TheoryData<EvaluateExpressionCommand> InvalidCharactersExpressions =>
     new TheoryData<EvaluateExpressionCommand>
     {
            new EvaluateExpressionCommand("A+1+3"),
            new EvaluateExpressionCommand("1+2=3"),
            new EvaluateExpressionCommand("1.1+2+3"),
            new EvaluateExpressionCommand("1 + 1"),
            new EvaluateExpressionCommand("1  +1"),
            new EvaluateExpressionCommand("1+1!2")

     };

     public static TheoryData<EvaluateExpressionCommand> SequentialOperatorsExpressions =>
     new TheoryData<EvaluateExpressionCommand>
     {
            new EvaluateExpressionCommand("++1+2"),
            new EvaluateExpressionCommand("--1+2"),
            new EvaluateExpressionCommand("//1+1"),
            new EvaluateExpressionCommand("**1+2"),
            new EvaluateExpressionCommand("1+-2"),
            new EvaluateExpressionCommand("1-*2"),
            new EvaluateExpressionCommand("1-*+/-2"),
            new EvaluateExpressionCommand("1-2+-")

     };


    public static TheoryData<EvaluateExpressionCommand> StartsOrEndsWithOperatorsExpressions =>
    new TheoryData<EvaluateExpressionCommand>
    {
            new EvaluateExpressionCommand("+1+2"),
            new EvaluateExpressionCommand("-1+2-"),
            new EvaluateExpressionCommand("/1+1"),
            new EvaluateExpressionCommand("*1+2"),
            new EvaluateExpressionCommand("1+2-"),
            new EvaluateExpressionCommand("1+2*"),
            new EvaluateExpressionCommand("+1-2-"),
            new EvaluateExpressionCommand("-"),
            new EvaluateExpressionCommand("+"),
            new EvaluateExpressionCommand("/"),
            new EvaluateExpressionCommand("*")

    };


    public static TheoryData<EvaluateExpressionCommand> ContainsOnlyDigitsExpressions =>
    new TheoryData<EvaluateExpressionCommand>
    {
            new EvaluateExpressionCommand("0"),
            new EvaluateExpressionCommand("1"),
            new EvaluateExpressionCommand("12"),
            new EvaluateExpressionCommand("123"),
            new EvaluateExpressionCommand("12345")

    };

    public static TheoryData<EvaluateExpressionCommand> InfinityExpressions =>
   new TheoryData<EvaluateExpressionCommand>
   {
            new EvaluateExpressionCommand("1/0"),
            new EvaluateExpressionCommand($"{double.MaxValue.ToString("0.#")}+{double.MaxValue.ToString("0.#")}")

   };

    public record ExpressionResultTest(EvaluateExpressionCommand command, double expectedResult);

    public static TheoryData<ExpressionResultTest> Expressions =>
  new TheoryData<ExpressionResultTest>
  {
            new ExpressionResultTest(new EvaluateExpressionCommand("4+5*2"),14.0),
            new ExpressionResultTest(new EvaluateExpressionCommand("4+5/2"),6.5),
            new ExpressionResultTest(new EvaluateExpressionCommand("4+5/2-1"),5.5),
            new ExpressionResultTest(new EvaluateExpressionCommand("1+1+1"),3.0),
            new ExpressionResultTest(new EvaluateExpressionCommand("1+2/3+4*5"),21.6666666),
            new ExpressionResultTest(new EvaluateExpressionCommand("1/2+3*4"),12.5),
            new ExpressionResultTest(new EvaluateExpressionCommand("1/2+3+4*5"),23.5),
            new ExpressionResultTest(new EvaluateExpressionCommand("1/2/6/34554+243-908/3453-1344*465767+134565/23454*1245-13243-344565+566754/54365*2342-324344*4543+45345-56564"),-2099822865.897338533165544),
            new ExpressionResultTest(new EvaluateExpressionCommand("1231/6546+657657-442*7867/234+65767-242+657-2342*675/342"),704356.93074383)


  };

    #endregion

    #region Tests
    [Theory(DisplayName = nameof(IsNullOrEmptyString))]
    [Trait("Application", "EvaluateExpressionCommandHandler")]
    [MemberData(nameof(NullOrEmptyValues))]
    public async Task IsNullOrEmptyString(EvaluateExpressionCommand command)
    {

        Func<Task> action = async () => { await _handler.Handle(command, default); };
        await action.Should().ThrowAsync<DomainValidationException>().WithMessage(ValidationResources.Null_Or_Empty_String);
    }


    [Theory(DisplayName = nameof(ContainsInvalidCharacters))]
    [Trait("Application", "EvaluateExpressionCommandHandler")]
    [MemberData(nameof(InvalidCharactersExpressions))]
    public async Task ContainsInvalidCharacters(EvaluateExpressionCommand command)
    {

        Func<Task> action = async () => { await _handler.Handle(command, default); };
        await action.Should().ThrowAsync<DomainValidationException>().WithMessage(ValidationResources.Invalid_Characters);
    }


    [Theory(DisplayName = nameof(ContainsSequentialOperators))]
    [Trait("Application", "EvaluateExpressionCommandHandler")]
    [MemberData(nameof(SequentialOperatorsExpressions))]
    public async Task ContainsSequentialOperators(EvaluateExpressionCommand command)
    {

        Func<Task> action = async () => { await _handler.Handle(command, default); };
        await action.Should().ThrowAsync<DomainValidationException>().WithMessage(ValidationResources.Consecutive_Operators);
    }


    [Theory(DisplayName = nameof(StartsOrEndsWithOperators))]
    [Trait("Application", "EvaluateExpressionCommandHandler")]
    [MemberData(nameof(StartsOrEndsWithOperatorsExpressions))]
    public async Task StartsOrEndsWithOperators(EvaluateExpressionCommand command)
    {

        Func<Task> action = async () => { await _handler.Handle(command, default); };
        await action.Should().ThrowAsync<DomainValidationException>().WithMessage(ValidationResources.Starts_Or_Ends_With_Operator);
    }


    [Theory(DisplayName = nameof(ContainsOnlyDigits))]
    [Trait("Application", "EvaluateExpressionCommandHandler")]
    [MemberData(nameof(ContainsOnlyDigitsExpressions))]
    public async Task ContainsOnlyDigits(EvaluateExpressionCommand command)
    {

        Func<Task> action = async () => { await _handler.Handle(command, default); };
        await action.Should().ThrowAsync<DomainValidationException>().WithMessage(ValidationResources.Contains_Only_Digits);
    }

    [Theory(DisplayName = nameof(EvaluateExpressions))]
    [Trait("Application", "EvaluateExpressionCommandHandler")]
    [MemberData(nameof(Expressions))]
    public async Task EvaluateExpressions(ExpressionResultTest testData)
    {

        var result = await _handler.Handle(testData.command, default);
        result.Should().BeApproximately(testData.expectedResult, 0.0001);
    }


    [Theory(DisplayName = nameof(EvaluateExpressionInfinity))]
    [Trait("Application", "EvaluateExpressionCommandHandler")]
    [MemberData(nameof(InfinityExpressions))]
    public async Task EvaluateExpressionInfinity(EvaluateExpressionCommand command)
    {

        Func<Task> action = async () => { await _handler.Handle(command, default); };
        await action.Should().ThrowAsync<ExpressionTreeEvaluationException>().WithMessage(ExpressionTreeEvaluationExpectionResource.Infinity_Value);
    }



    #endregion
}
