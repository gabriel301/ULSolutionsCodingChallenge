using FluentAssertions;
using MediatR;
using NSubstitute;
using UL.Application.DomainServices;
using UL.Application.ExpressionTree.Command;
using UL.Application.ExpressionTree.Handlers;
using UL.Domain.Events.ExpressionTree.Created;
using UL.Domain.Events.ExpressionTree.Evaluated;
using UL.Domain.Exceptions;
using UL.Domain.Exceptions.ExpressionTree;
using UL.Domain.Resources;
using UL.Domain.Services.Abstraction;
using UL.Shared.Resources;

namespace UL.Tests.Application.Application.Handler;
public class EvaluateTreeExpressionCommandHandlerTest
{
    private readonly EvaluateTreeExpressionCommandHandler _handler;
    private readonly IPublisher _publisherMock;
    private readonly IOperationService _operationService;

    #region Setup
    public EvaluateTreeExpressionCommandHandlerTest()
    {
        _publisherMock = Substitute.For<IPublisher>();
        _publisherMock.Publish(Arg.Any<ExpressionTreeCreatedEvent>(), Arg.Any<CancellationToken>());
        _publisherMock.Publish(Arg.Any<ExpressionTreeEvaluatedEvent>(), Arg.Any<CancellationToken>());
        _operationService = new BasicArithmeticOperationService();
        _handler = new EvaluateTreeExpressionCommandHandler(_publisherMock, _operationService);
    }

    #endregion

    #region Theory Data
    public static TheoryData<EvaluateTreeExpressionCommand> NullOrEmptyValues =>
     new TheoryData<EvaluateTreeExpressionCommand>
     {
            new EvaluateTreeExpressionCommand(null),
            new EvaluateTreeExpressionCommand(""),
            new EvaluateTreeExpressionCommand("  "),
            new EvaluateTreeExpressionCommand("     ")

     };


    public static TheoryData<EvaluateTreeExpressionCommand> InvalidCharactersExpressions =>
     new TheoryData<EvaluateTreeExpressionCommand>
     {
            new EvaluateTreeExpressionCommand("A+1+3"),
            new EvaluateTreeExpressionCommand("1+2=3"),
            new EvaluateTreeExpressionCommand("1.1+2+3"),
            new EvaluateTreeExpressionCommand("1 + 1"),
            new EvaluateTreeExpressionCommand("1  +1"),
            new EvaluateTreeExpressionCommand("1+1!2")

     };

    public static TheoryData<EvaluateTreeExpressionCommand> SequentialOperatorsExpressions =>
    new TheoryData<EvaluateTreeExpressionCommand>
    {
            new EvaluateTreeExpressionCommand("++1+2"),
            new EvaluateTreeExpressionCommand("--1+2"),
            new EvaluateTreeExpressionCommand("//1+1"),
            new EvaluateTreeExpressionCommand("**1+2"),
            new EvaluateTreeExpressionCommand("1+-2"),
            new EvaluateTreeExpressionCommand("1-*2"),
            new EvaluateTreeExpressionCommand("1-*+/-2"),
            new EvaluateTreeExpressionCommand("1-2+-")

    };


    public static TheoryData<EvaluateTreeExpressionCommand> StartsOrEndsWithOperatorsExpressions =>
    new TheoryData<EvaluateTreeExpressionCommand>
    {
            new EvaluateTreeExpressionCommand("+1+2"),
            new EvaluateTreeExpressionCommand("-1+2-"),
            new EvaluateTreeExpressionCommand("/1+1"),
            new EvaluateTreeExpressionCommand("*1+2"),
            new EvaluateTreeExpressionCommand("1+2-"),
            new EvaluateTreeExpressionCommand("1+2*"),
            new EvaluateTreeExpressionCommand("+1-2-"),
            new EvaluateTreeExpressionCommand("-"),
            new EvaluateTreeExpressionCommand("+"),
            new EvaluateTreeExpressionCommand("/"),
            new EvaluateTreeExpressionCommand("*")

    };


    public static TheoryData<EvaluateTreeExpressionCommand> ContainsOnlyDigitsExpressions =>
    new TheoryData<EvaluateTreeExpressionCommand>
    {
            new EvaluateTreeExpressionCommand("0"),
            new EvaluateTreeExpressionCommand("1"),
            new EvaluateTreeExpressionCommand("12"),
            new EvaluateTreeExpressionCommand("123"),
            new EvaluateTreeExpressionCommand("12345")

    };

    public static TheoryData<EvaluateTreeExpressionCommand> InfinityExpressions =>
   new TheoryData<EvaluateTreeExpressionCommand>
   {
            new EvaluateTreeExpressionCommand("1/0"),
            new EvaluateTreeExpressionCommand($"{double.MaxValue.ToString("0.#")}+{double.MaxValue.ToString("0.#")}")

   };

    public record ExpressionResultTest(EvaluateTreeExpressionCommand command, double expectedResult);

    public static TheoryData<ExpressionResultTest> Expressions =>
  new TheoryData<ExpressionResultTest>
  {
            new ExpressionResultTest(new EvaluateTreeExpressionCommand("4+5*2"),14.0),
            new ExpressionResultTest(new EvaluateTreeExpressionCommand("4+5/2"),6.5),
            new ExpressionResultTest(new EvaluateTreeExpressionCommand("4+5/2-1"),5.5),
            new ExpressionResultTest(new EvaluateTreeExpressionCommand("1+1+1"),3.0),
            new ExpressionResultTest(new EvaluateTreeExpressionCommand("1+2/3+4*5"),21.6666666),
            new ExpressionResultTest(new EvaluateTreeExpressionCommand("1/2+3*4"),12.5),
            new ExpressionResultTest(new EvaluateTreeExpressionCommand("1/2+3+4*5"),23.5),
            new ExpressionResultTest(new EvaluateTreeExpressionCommand("1/2/6/34554+243-908/3453-1344*465767+134565/23454*1245-13243-344565+566754/54365*2342-324344*4543+45345-56564"),-2099822865.897338533165544),
            new ExpressionResultTest(new EvaluateTreeExpressionCommand("1231/6546+657657-442*7867/234+65767-242+657-2342*675/342"),704356.93074383)


  };

    #endregion 

    #region Tests
    [Theory(DisplayName = nameof(IsNullOrEmptyString))]
    [Trait("Application", "EvaluateTreeExpressionCommandHandler")]
    [MemberData(nameof(NullOrEmptyValues))]
    public async Task IsNullOrEmptyString(EvaluateTreeExpressionCommand command)
    {

        Func<Task> action = async () => { await _handler.Handle(command, default); };
        await action.Should().ThrowAsync<DomainValidationException>().WithMessage(ValidationResources.Null_Or_Empty_String);
    }


    [Theory(DisplayName = nameof(ContainsInvalidCharacters))]
    [Trait("Application", "EvaluateTreeExpressionCommandHandler")]
    [MemberData(nameof(InvalidCharactersExpressions))]
    public async Task ContainsInvalidCharacters(EvaluateTreeExpressionCommand command)
    {

        Func<Task> action = async () => { await _handler.Handle(command, default); };
        await action.Should().ThrowAsync<DomainValidationException>().WithMessage(ValidationResources.Invalid_Characters);
    }


    [Theory(DisplayName = nameof(ContainsSequentialOperators))]
    [Trait("Application", "EvaluateTreeExpressionCommandHandler")]
    [MemberData(nameof(SequentialOperatorsExpressions))]
    public async Task ContainsSequentialOperators(EvaluateTreeExpressionCommand command)
    {

        Func<Task> action = async () => { await _handler.Handle(command, default); };
        await action.Should().ThrowAsync<DomainValidationException>().WithMessage(ValidationResources.Consecutive_Operators);
    }


    [Theory(DisplayName = nameof(StartsOrEndsWithOperators))]
    [Trait("Application", "EvaluateTreeExpressionCommandHandler")]
    [MemberData(nameof(StartsOrEndsWithOperatorsExpressions))]
    public async Task StartsOrEndsWithOperators(EvaluateTreeExpressionCommand command)
    {

        Func<Task> action = async () => { await _handler.Handle(command, default); };
        await action.Should().ThrowAsync<DomainValidationException>().WithMessage(ValidationResources.Starts_Or_Ends_With_Operator);
    }


    [Theory(DisplayName = nameof(ContainsOnlyDigits))]
    [Trait("Application", "EvaluateTreeExpressionCommandHandler")]
    [MemberData(nameof(ContainsOnlyDigitsExpressions))]
    public async Task ContainsOnlyDigits(EvaluateTreeExpressionCommand command)
    {

        Func<Task> action = async () => { await _handler.Handle(command, default); };
        await action.Should().ThrowAsync<DomainValidationException>().WithMessage(ValidationResources.Contains_Only_Digits);
    }

    [Theory(DisplayName = nameof(EvaluateExpressions))]
    [Trait("Application", "EvaluateTreeExpressionCommandHandler")]
    [MemberData(nameof(Expressions))]
    public async Task EvaluateExpressions(ExpressionResultTest testData)
    {

        var result = await _handler.Handle(testData.command, default);
        result.Should().BeApproximately(testData.expectedResult, 0.0001);
    }


    [Theory(DisplayName = nameof(EvaluateExpressionInfinity))]
    [Trait("Application", "EvaluateTreeExpressionCommandHandler")]
    [MemberData(nameof(InfinityExpressions))]
    public async Task EvaluateExpressionInfinity(EvaluateTreeExpressionCommand command)
    {

        Func<Task> action = async () => { await _handler.Handle(command, default); };
        await action.Should().ThrowAsync<ExpressionTreeEvaluationException>().WithMessage(ExpressionTreeEvaluationExceptionResource.Infinity_Value);
    }



    #endregion
}
