using FluentValidation.TestHelper;
using UL.Application.ArithmeticExpression.Command;
using UL.Application.ArithmeticExpression.Validators;
using UL.Shared.Resources;

namespace UL.Tests.Application.Application.Validation;
public class ArithmeticExpressionCommandValidatorTest
{
    #region Setup
    private ArithmeticExpressionCommandValidator _validator;
    public ArithmeticExpressionCommandValidatorTest()
    {
        _validator = new ArithmeticExpressionCommandValidator();
    }

    #endregion

    #region Theory Data
    public static TheoryData<EvaluateArithmeticExpressionCommand> EmptyValues =>
     new TheoryData<EvaluateArithmeticExpressionCommand>
     {
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

    #endregion

    #region Tests
    [Fact(DisplayName = nameof(IsNullString))]
    [Trait("Application", "ArithmeticExpressionCommandValidator")]
    public void IsNullString()
    {

        var command = new EvaluateArithmeticExpressionCommand(null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveAnyValidationError()
              .WithErrorCode(ValidationErrorCodesResource.Not_Null);

    }

    [Theory(DisplayName = nameof(IsEmptyString))]
    [Trait("Application", "ArithmeticExpressionCommandValidator")]
    [MemberData(nameof(EmptyValues))]
    public void IsEmptyString(EvaluateArithmeticExpressionCommand command)
    {

        var result = _validator.TestValidate(command);
        result.ShouldHaveAnyValidationError()
              .WithErrorCode(ValidationErrorCodesResource.Not_Empty);

    }

    [Theory(DisplayName = nameof(ContainsInvalidCharacters))]
    [Trait("Application", "ArithmeticExpressionCommandValidator")]
    [MemberData(nameof(InvalidCharactersExpressions))]
    public void ContainsInvalidCharacters(EvaluateArithmeticExpressionCommand command)
    {

        var result = _validator.TestValidate(command);
        result.ShouldHaveAnyValidationError()
              .WithErrorCode(ValidationErrorCodesResource.Invalid_Characters);

    }


    [Theory(DisplayName = nameof(SequentialOperatorsExpressions))]
    [Trait("Application", "ArithmeticExpressionCommandValidator")]
    [MemberData(nameof(SequentialOperatorsExpressions))]
    public void ContainsSequentialOperators(EvaluateArithmeticExpressionCommand command)
    {

        var result = _validator.TestValidate(command);
        result.ShouldHaveAnyValidationError()
              .WithErrorCode(ValidationErrorCodesResource.Consecutive_Operators);

    }

    [Theory(DisplayName = nameof(StartsOrEndsWithOperatorsExpressions))]
    [Trait("Application", "ArithmeticExpressionCommandValidator")]
    [MemberData(nameof(StartsOrEndsWithOperatorsExpressions))]
    public void StartsOrEndsWithOperators(EvaluateArithmeticExpressionCommand command)
    {

        var result = _validator.TestValidate(command);
        result.ShouldHaveAnyValidationError()
              .WithErrorCode(ValidationErrorCodesResource.Starts_Or_Ends_With_Non_Digits);

    }

    [Theory(DisplayName = nameof(ContainsOnlyDigitsExpressions))]
    [Trait("Application", "ArithmeticExpressionCommandValidator")]
    [MemberData(nameof(ContainsOnlyDigitsExpressions))]
    public void ConatinsOnlyDigits(EvaluateArithmeticExpressionCommand command)
    {

        var result = _validator.TestValidate(command);
        result.ShouldHaveAnyValidationError()
              .WithErrorCode(ValidationErrorCodesResource.Contains_Only_Digits);

    }

    #endregion
}
