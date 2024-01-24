using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UL.Application.Expression.Command;
using UL.Shared.RegexPatterns;
using UL.Shared.Resources;

namespace UL.Application.Expression.Validators;
public class ExpressionCommandValidator : AbstractValidator<EvaluateExpressionCommand>
{
    public ExpressionCommandValidator()
    {
        RuleFor(command => command.expression)
            .NotNull()
            .WithErrorCode(ValidationErrorCodesResource.Not_Null);
        
        RuleFor(command => command.expression)
            .NotEmpty()
            .WithErrorCode(ValidationErrorCodesResource.Not_Empty);

        RuleFor(command => command)
            .Must(command => command.expression != null && !Regex.Match(command.expression, ValidationPatterns.INVALID_CHARACTERS).Success)
            .WithMessage(ValidationResources.Invalid_Characters)
            .WithErrorCode(ValidationErrorCodesResource.Invalid_Characters);

        RuleFor(command => command).NotNull().NotEmpty()
            .Must(command => command.expression != null && !Regex.Match(command.expression, ValidationPatterns.CONSECUTIVE_OPERATORS).Success)
            .WithMessage(ValidationResources.Consecutive_Operators)
            .WithErrorCode(ValidationErrorCodesResource.Consecutive_Operators);

        RuleFor(command => command).NotNull().NotEmpty()
           .Must(command => command.expression != null && !Regex.Match(command.expression, ValidationPatterns.STARTS_OR_ENDS_WITH_NON_DIGITS).Success)
           .WithMessage(ValidationResources.Starts_Or_Ends_With_Operator)
           .WithErrorCode(ValidationErrorCodesResource.Starts_Or_Ends_With_Non_Digits);

        RuleFor(command => command).NotNull().NotEmpty()
         .Must(command => command.expression != null && !Regex.Match(command.expression, ValidationPatterns.CONTAINS_ONLY_DIGITS).Success)
         .WithMessage(ValidationResources.Contains_Only_Digits)
         .WithErrorCode(ValidationErrorCodesResource.Contains_Only_Digits);
    }
}
