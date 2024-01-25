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
    public static TheoryData<EvaluateTreeExpressionCommand> EmptyValues =>
     new TheoryData<EvaluateTreeExpressionCommand>
     {
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

    #endregion

    #region Tests
    [Fact (DisplayName = nameof(IsNullString))]
    [Trait("Application", "ExpressionCommandValidator")]
    public void IsNullString()
    {

        var command = new EvaluateTreeExpressionCommand(null);

        var result = _validator.TestValidate(command);
        result.ShouldHaveAnyValidationError()
              .WithErrorCode(ValidationErrorCodesResource.Not_Null);

    }

    [Theory (DisplayName = nameof(IsEmptyString))]
    [Trait("Application", "ExpressionCommandValidator")]
    [MemberData(nameof(EmptyValues))]
    public void IsEmptyString(EvaluateTreeExpressionCommand command)
    {

        var result = _validator.TestValidate(command);
        result.ShouldHaveAnyValidationError()
              .WithErrorCode(ValidationErrorCodesResource.Not_Empty);

    }

    [Theory(DisplayName = nameof(ContainsInvalidCharacters))]
    [Trait("Application", "ExpressionCommandValidator")]
    [MemberData(nameof(InvalidCharactersExpressions))]
    public void ContainsInvalidCharacters(EvaluateTreeExpressionCommand command)
    {

        var result = _validator.TestValidate(command);
        result.ShouldHaveAnyValidationError()
              .WithErrorCode(ValidationErrorCodesResource.Invalid_Characters);

    }


    [Theory(DisplayName = nameof(SequentialOperatorsExpressions))]
    [Trait("Application", "ExpressionCommandValidator")]
    [MemberData(nameof(SequentialOperatorsExpressions))]
    public void ContainsSequentialOperators(EvaluateTreeExpressionCommand command)
    {

        var result = _validator.TestValidate(command);
        result.ShouldHaveAnyValidationError()
              .WithErrorCode(ValidationErrorCodesResource.Consecutive_Operators);

    }

    [Theory(DisplayName = nameof(StartsOrEndsWithOperatorsExpressions))]
    [Trait("Application", "ExpressionCommandValidator")]
    [MemberData(nameof(StartsOrEndsWithOperatorsExpressions))]
    public void StartsOrEndsWithOperators(EvaluateTreeExpressionCommand command)
    {

        var result = _validator.TestValidate(command);
        result.ShouldHaveAnyValidationError()
              .WithErrorCode(ValidationErrorCodesResource.Starts_Or_Ends_With_Non_Digits);

    }

    [Theory(DisplayName = nameof(ContainsOnlyDigitsExpressions))]
    [Trait("Application", "ExpressionCommandValidator")]
    [MemberData(nameof(ContainsOnlyDigitsExpressions))]
    public void ConatinsOnlyDigits(EvaluateTreeExpressionCommand command)
    {

        var result = _validator.TestValidate(command);
        result.ShouldHaveAnyValidationError()
              .WithErrorCode(ValidationErrorCodesResource.Contains_Only_Digits);

    }

    #endregion
}
