using FluentAssertions;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using UL.Application.Expression.Command;
using UL.Application.Expression.Validators;
using UL.Domain.Exceptions;
using UL.Shared.Resources;

namespace UL.Tests.Application.Application.Validation;
public class ExpressionCommandValidatorTest
{
    private ExpressionCommandValidator _validator;

    #region Setup
    public ExpressionCommandValidatorTest()
    {
        _validator = new ExpressionCommandValidator();
    }

    #endregion

    #region Theory Data
    public static TheoryData<EvaluateExpressionCommand> EmptyValues =>
     new TheoryData<EvaluateExpressionCommand>
     {
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

    #endregion

    #region Tests
    [Fact (DisplayName = nameof(IsNullString))]
    [Trait("Application", "ExpressionCommandValidator")]
    public void IsNullString()
    {

        var command = new EvaluateExpressionCommand(null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveAnyValidationError()
              .WithErrorCode(ValidationErrorCodesResource.Not_Null);

    }

    [Theory (DisplayName = nameof(IsEmptyString))]
    [Trait("Application", "ExpressionCommandValidator")]
    [MemberData(nameof(EmptyValues))]
    public void IsEmptyString(EvaluateExpressionCommand command)
    {

        var result = _validator.TestValidate(command);
        result.ShouldHaveAnyValidationError()
              .WithErrorCode(ValidationErrorCodesResource.Not_Empty);

    }

    [Theory(DisplayName = nameof(ContainsInvalidCharacters))]
    [Trait("Application", "ExpressionCommandValidator")]
    [MemberData(nameof(InvalidCharactersExpressions))]
    public void ContainsInvalidCharacters(EvaluateExpressionCommand command)
    {

        var result = _validator.TestValidate(command);
        result.ShouldHaveAnyValidationError()
              .WithErrorCode(ValidationErrorCodesResource.Invalid_Characters);

    }


    [Theory(DisplayName = nameof(SequentialOperatorsExpressions))]
    [Trait("Application", "ExpressionCommandValidator")]
    [MemberData(nameof(SequentialOperatorsExpressions))]
    public void ContainsSequentialOperators(EvaluateExpressionCommand command)
    {

        var result = _validator.TestValidate(command);
        result.ShouldHaveAnyValidationError()
              .WithErrorCode(ValidationErrorCodesResource.Consecutive_Operators);

    }

    [Theory(DisplayName = nameof(StartsOrEndsWithOperatorsExpressions))]
    [Trait("Application", "ExpressionCommandValidator")]
    [MemberData(nameof(StartsOrEndsWithOperatorsExpressions))]
    public void StartsOrEndsWithOperators(EvaluateExpressionCommand command)
    {

        var result = _validator.TestValidate(command);
        result.ShouldHaveAnyValidationError()
              .WithErrorCode(ValidationErrorCodesResource.Starts_Or_Ends_With_Non_Digits);

    }

    [Theory(DisplayName = nameof(ContainsOnlyDigitsExpressions))]
    [Trait("Application", "ExpressionCommandValidator")]
    [MemberData(nameof(ContainsOnlyDigitsExpressions))]
    public void ConatinsOnlyDigits(EvaluateExpressionCommand command)
    {

        var result = _validator.TestValidate(command);
        result.ShouldHaveAnyValidationError()
              .WithErrorCode(ValidationErrorCodesResource.Contains_Only_Digits);

    }

    #endregion
}
