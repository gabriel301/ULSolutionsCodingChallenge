using Domain.Exceptions;
using Domain.Interfaces;
using Shared.RegexPatterns;
using Shared.ReShared.Resources.Validationsoures.Validation;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Domain.Entities.Expression;

public sealed class ArithmeticExpression : IDisposable
{

    public string ExpresionString { get; private set; }

    public IExpressionTree? ExpressionTree { get; private set; }

    private bool disposed = false;

    private ArithmeticExpression(string expressionString)
    {
        Validate(expressionString);
        ExpresionString = expressionString;
        

    }

    public static ArithmeticExpression Create(string expressionString, IExpressionTree expressionTree) 
    {
        ArithmeticExpression expression = new ArithmeticExpression(expressionString);

        

        if (expressionTree is null) 
        {
            throw new DomainException(ValidationResources.Expression_Tree_Null);
        }
        expression.ExpressionTree = expressionTree;



        return expression;
    }

    private void Validate(string expressionString)
    {
        if (expressionString is null || expressionString.Trim().Equals(string.Empty))
        {
            throw new DomainException(ValidationResources.Null_Or_Empty_String);
        }

        if (Regex.Match(expressionString, ValidationPatterns.INVALID_CHARACTERS).Success)
        {
            throw new DomainException(expressionString, ValidationResources.Invalid_Characters);
        }

        if (Regex.Match(expressionString, ValidationPatterns.CONSECUTIVE_OPERATORS).Success)
        {
            throw new DomainException(expressionString, ValidationResources.Consecutive_Operators);
        }

        if (Regex.Match(expressionString, ValidationPatterns.STARTS_OR_ENDS_WITH_NON_DIGITS).Success)
        {
            throw new DomainException(expressionString, ValidationResources.Starts_Or_Ends_With_Operator);
        }

        if (Regex.Match(expressionString, ValidationPatterns.CONTAINS_ONLY_DIGITS).Success)
        {
            throw new DomainException(expressionString, ValidationResources.Contains_Only_Digits);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                this.ExpressionTree!.Dispose();
                this.ExpresionString = string.Empty;
            }
            disposed = true;
        }

    }

    //Destructor
    ~ArithmeticExpression()
    {
        Dispose(false);
    }
}

