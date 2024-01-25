using FluentAssertions;
using MediatR;
using NSubstitute;
using UL.Application.ArithmeticExpression.Command;
using UL.Application.ArithmeticExpression.Handlers;
using UL.Application.DomainServices;
using UL.Domain.Events.ArithmeticExpression.Created;
using UL.Domain.Events.ArithmeticExpression.Evaluated;
using UL.Domain.Exceptions;
using UL.Domain.Exceptions.ArithmeticExpression;
using UL.Domain.Resources;
using UL.Domain.Services.Abstraction;
using UL.Shared.Resources;

namespace UL.Tests.Application.Application.Handler;
public class EvaluateArithmeticExpressionCommandHandlerTest
{
    private readonly EvaluateArithmeticExpressionCommandHandler _handler;
    private readonly IPublisher _publisherMock;
    private readonly IOperationService _operationService;

    #region Setup
    public EvaluateArithmeticExpressionCommandHandlerTest()
    {
        _publisherMock = Substitute.For<IPublisher>();
        _publisherMock.Publish(Arg.Any<ArithmeticExpressionCreatedEvent>(), Arg.Any<CancellationToken>());
        _publisherMock.Publish(Arg.Any<ArithmeticExpressionEvaluatedEvent>(), Arg.Any<CancellationToken>());
        _operationService = new BasicArithmeticOperationService();
        _handler = new EvaluateArithmeticExpressionCommandHandler(_publisherMock, _operationService);
    }

    #endregion

    #region Theory Data
    public static TheoryData<EvaluateArithmeticExpressionCommand> NullOrEmptyValues =>
     new TheoryData<EvaluateArithmeticExpressionCommand>
     {
            new EvaluateArithmeticExpressionCommand(null),
            new EvaluateArithmeticExpressionCommand(""),
            new EvaluateArithmeticExpressionCommand("  "),
            new EvaluateArithmeticExpressionCommand("     ")

     };


    public static TheoryData<EvaluateArithmeticExpressionCommand> InvalidCharactersExpressions =>
     new TheoryData<EvaluateArithmeticExpressionCommand>
     {
            new EvaluateArithmeticExpressionCommand("A+1+3"),
            new EvaluateArithmeticExpressionCommand("1+2=3"),
            new EvaluateArithmeticExpressionCommand("1.1+2+3"),
            new EvaluateArithmeticExpressionCommand("1 + 1"),
            new EvaluateArithmeticExpressionCommand("1  +1"),
            new EvaluateArithmeticExpressionCommand("1+1!2")

     };

    public static TheoryData<EvaluateArithmeticExpressionCommand> SequentialOperatorsExpressions =>
    new TheoryData<EvaluateArithmeticExpressionCommand>
    {
            new EvaluateArithmeticExpressionCommand("++1+2"),
            new EvaluateArithmeticExpressionCommand("--1+2"),
            new EvaluateArithmeticExpressionCommand("//1+1"),
            new EvaluateArithmeticExpressionCommand("**1+2"),
            new EvaluateArithmeticExpressionCommand("1+-2"),
            new EvaluateArithmeticExpressionCommand("1-*2"),
            new EvaluateArithmeticExpressionCommand("1-*+/-2"),
            new EvaluateArithmeticExpressionCommand("1-2+-")

    };


    public static TheoryData<EvaluateArithmeticExpressionCommand> StartsOrEndsWithOperatorsExpressions =>
    new TheoryData<EvaluateArithmeticExpressionCommand>
    {
            new EvaluateArithmeticExpressionCommand("+1+2"),
            new EvaluateArithmeticExpressionCommand("-1+2-"),
            new EvaluateArithmeticExpressionCommand("/1+1"),
            new EvaluateArithmeticExpressionCommand("*1+2"),
            new EvaluateArithmeticExpressionCommand("1+2-"),
            new EvaluateArithmeticExpressionCommand("1+2*"),
            new EvaluateArithmeticExpressionCommand("+1-2-"),
            new EvaluateArithmeticExpressionCommand("-"),
            new EvaluateArithmeticExpressionCommand("+"),
            new EvaluateArithmeticExpressionCommand("/"),
            new EvaluateArithmeticExpressionCommand("*")

    };


    public static TheoryData<EvaluateArithmeticExpressionCommand> ContainsOnlyDigitsExpressions =>
    new TheoryData<EvaluateArithmeticExpressionCommand>
    {
            new EvaluateArithmeticExpressionCommand("0"),
            new EvaluateArithmeticExpressionCommand("1"),
            new EvaluateArithmeticExpressionCommand("12"),
            new EvaluateArithmeticExpressionCommand("123"),
            new EvaluateArithmeticExpressionCommand("12345")

    };

    public static TheoryData<EvaluateArithmeticExpressionCommand> InfinityExpressions =>
   new TheoryData<EvaluateArithmeticExpressionCommand>
   {
            new EvaluateArithmeticExpressionCommand("1/0"),
            new EvaluateArithmeticExpressionCommand($"{double.MaxValue.ToString("0.#")}+{double.MaxValue.ToString("0.#")}")

   };

    public record ExpressionResultTest(EvaluateArithmeticExpressionCommand command, double expectedResult);

    public static TheoryData<ExpressionResultTest> Expressions =>
  new TheoryData<ExpressionResultTest>
  {
            new ExpressionResultTest(new EvaluateArithmeticExpressionCommand("4+5*2"),14.0),
            new ExpressionResultTest(new EvaluateArithmeticExpressionCommand("4+5/2"),6.5),
            new ExpressionResultTest(new EvaluateArithmeticExpressionCommand("4+5/2-1"),5.5),
            new ExpressionResultTest(new EvaluateArithmeticExpressionCommand("1+1+1"),3.0),
            new ExpressionResultTest(new EvaluateArithmeticExpressionCommand("1+2/3+4*5"),21.6666666),
            new ExpressionResultTest(new EvaluateArithmeticExpressionCommand("1/2+3*4"),12.5),
            new ExpressionResultTest(new EvaluateArithmeticExpressionCommand("1/2+3+4*5"),23.5),
            new ExpressionResultTest(new EvaluateArithmeticExpressionCommand("1/2/6/34554+243-908/3453-1344*465767+134565/23454*1245-13243-344565+566754/54365*2342-324344*4543+45345-56564"),-2099822865.897338533165544),
            new ExpressionResultTest(new EvaluateArithmeticExpressionCommand("1231/6546+657657-442*7867/234+65767-242+657-2342*675/342"),704356.93074383)


  };

    #endregion

    #region Tests
    [Theory(DisplayName = nameof(IsNullOrEmptyString))]
    [Trait("Application", "EvaluateArithmeticExpressionCommandHandler")]
    [MemberData(nameof(NullOrEmptyValues))]
    public async Task IsNullOrEmptyString(EvaluateArithmeticExpressionCommand command)
    {

        Func<Task> action = async () => { await _handler.Handle(command, default); };
        await action.Should().ThrowAsync<DomainValidationException>().WithMessage(ValidationResources.Null_Or_Empty_String);
    }


    [Theory(DisplayName = nameof(ContainsInvalidCharacters))]
    [Trait("Application", "EvaluateArithmeticExpressionCommandHandler")]
    [MemberData(nameof(InvalidCharactersExpressions))]
    public async Task ContainsInvalidCharacters(EvaluateArithmeticExpressionCommand command)
    {

        Func<Task> action = async () => { await _handler.Handle(command, default); };
        await action.Should().ThrowAsync<DomainValidationException>().WithMessage(ValidationResources.Invalid_Characters);
    }


    [Theory(DisplayName = nameof(ContainsSequentialOperators))]
    [Trait("Application", "EvaluateArithmeticExpressionCommandHandler")]
    [MemberData(nameof(SequentialOperatorsExpressions))]
    public async Task ContainsSequentialOperators(EvaluateArithmeticExpressionCommand command)
    {

        Func<Task> action = async () => { await _handler.Handle(command, default); };
        await action.Should().ThrowAsync<DomainValidationException>().WithMessage(ValidationResources.Consecutive_Operators);
    }


    [Theory(DisplayName = nameof(StartsOrEndsWithOperators))]
    [Trait("Application", "EvaluateArithmeticExpressionCommandHandler")]
    [MemberData(nameof(StartsOrEndsWithOperatorsExpressions))]
    public async Task StartsOrEndsWithOperators(EvaluateArithmeticExpressionCommand command)
    {

        Func<Task> action = async () => { await _handler.Handle(command, default); };
        await action.Should().ThrowAsync<DomainValidationException>().WithMessage(ValidationResources.Starts_Or_Ends_With_Operator);
    }


    [Theory(DisplayName = nameof(ContainsOnlyDigits))]
    [Trait("Application", "EvaluateArithmeticExpressionCommandHandler")]
    [MemberData(nameof(ContainsOnlyDigitsExpressions))]
    public async Task ContainsOnlyDigits(EvaluateArithmeticExpressionCommand command)
    {

        Func<Task> action = async () => { await _handler.Handle(command, default); };
        await action.Should().ThrowAsync<DomainValidationException>().WithMessage(ValidationResources.Contains_Only_Digits);
    }

    [Theory(DisplayName = nameof(EvaluateExpressions))]
    [Trait("Application", "EvaluateArithmeticExpressionCommandHandler")]
    [MemberData(nameof(Expressions))]
    public async Task EvaluateExpressions(ExpressionResultTest testData)
    {

        var result = await _handler.Handle(testData.command, default);
        result.Should().BeApproximately(testData.expectedResult, 0.0001);
    }


    [Theory(DisplayName = nameof(EvaluateExpressionInfinity))]
    [Trait("Application", "EvaluateArithmeticExpressionCommandHandler")]
    [MemberData(nameof(InfinityExpressions))]
    public async Task EvaluateExpressionInfinity(EvaluateArithmeticExpressionCommand command)
    {

        Func<Task> action = async () => { await _handler.Handle(command, default); };
        await action.Should().ThrowAsync<ArithmeticExpressionEvaluationException>().WithMessage(ArithmeticExpressionEvaluationExceptionResource.Infinity_Value);
    }



    #endregion
}
