using Domain.Exceptions;
using Shared.RegexPatterns;
using Shared.ReShared.Resources.Validationsoures.Validation;
using System.Text.RegularExpressions;

namespace Domain.Entities;

public sealed class ArithmeticExpression
{

    public string ExpresionString { get; private set; }

    public ArithmeticExpression(string expressionString)
    {
        Validate(expressionString);
        ExpresionString = expressionString;
    }

    private void Validate(string expressionString)
    {
        if (expressionString is null || expressionString.Trim().Equals(string.Empty))
        {
            throw new DomainException(ValidationResources.ResourceManager.GetString("Null_Or_Empty_String"));
        }

        if (Regex.Match(expressionString, ValidationPatterns.INVALID_CHARACTERS).Success)
        {
            throw new DomainException(ValidationResources.ResourceManager.GetString("Invalid_Characters"));
        }

        if (Regex.Match(expressionString, ValidationPatterns.CONSECUTIVE_OPERATORS).Success)
        {
            throw new DomainException(ValidationResources.ResourceManager.GetString("Consecutive_Operators"));
        }

        if (Regex.Match(expressionString, ValidationPatterns.STARTS_OR_ENDS_WITH_NON_DIGITS).Success)
        {
            throw new DomainException(ValidationResources.ResourceManager.GetString("Starts_Or_Ends_With_Operator"));
        }

        if (Regex.Match(expressionString, ValidationPatterns.CONTAINS_ONLY_DIGITS).Success)
        {
            throw new DomainException(ValidationResources.ResourceManager.GetString("Contains_Only_Digits"));
        }
    }
}

