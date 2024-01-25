using System.Text.RegularExpressions;
using UL.Domain.Entities.Abstraction;
using UL.Domain.Events.ArithmeticExpression.Created;
using UL.Domain.Events.ArithmeticExpression.Evaluated;
using UL.Domain.Exceptions;
using UL.Domain.Exceptions.ArithmeticExpression;
using UL.Domain.Resources;
using UL.Domain.Services.Abstraction;
using UL.Shared.RegexPatterns;
using UL.Shared.Resources;

namespace UL.Domain.Entities.ArithmeticExpression;
public sealed class ArithmeticExpressionEvaluator : Entity
{

    private bool isEvaluated = false;

    public ArithmeticExpressionEvaluator(string expression)
    {
        Validate(expression);
        Expression = expression;

    }

    public string Expression { get; private set; }
    public double Evaluation { get; private set; }



    public static ArithmeticExpressionEvaluator Create(string expression)
    {
        ArithmeticExpressionEvaluator newExpression = new ArithmeticExpressionEvaluator(expression);

        ArithmeticExpressionCreatedEventData eventData = new ArithmeticExpressionCreatedEventData(newExpression.Expression);

        ArithmeticExpressionCreatedEvent domainEvent = new ArithmeticExpressionCreatedEvent(eventData);

        newExpression.AddDomainEvent(domainEvent);

        return newExpression;
    }

    public double Evaluate(IOperationService operationService)
    {
        if (!isEvaluated)
        {
            Evaluation = EvaluateExpression(operationService);
            ArithmeticExpressionEvaluatedEventData eventData = new ArithmeticExpressionEvaluatedEventData(Expression, Evaluation);

            ArithmeticExpressionEvaluatedEvent domainEvent = new ArithmeticExpressionEvaluatedEvent(eventData);

            AddDomainEvent(domainEvent);
        }

        return Evaluation;
    }

    private double EvaluateExpression(IOperationService operationService)
    {

        Stack<double> operandStack = new Stack<double>();
        Stack<string> operatorStack = new Stack<string>();
        var evaluatedExpression = Expression.Substring(0);

        int precedences = Regex.Matches(evaluatedExpression, EvaluationPatterns.MATCH_ALL_MULTIPLICATION_OR_DIVISION).Count;
        while (evaluatedExpression.Length > 0)
        {
            var match = Regex.Match(evaluatedExpression, EvaluationPatterns.FIRST_PART_DIGIT);
            if (match.Length > 0)
            {
                operandStack.Push(double.Parse(match.Value));
                evaluatedExpression = evaluatedExpression.Substring(match.Index + match.Length);
                continue;
            }

            match = Regex.Match(evaluatedExpression, EvaluationPatterns.FIRST_PART_SUM_OR_SUBTRACTION);

            if (match.Length > 0)
            {
                operatorStack.Push(match.Value);
                evaluatedExpression = evaluatedExpression.Substring(match.Index + match.Length);
                continue;
            }

            match = Regex.Match(evaluatedExpression, EvaluationPatterns.FIRST_PART_MULTIPLICATION_OR_DIVISION);

            if (match.Length > 0)
            {
                operatorStack.Push(match.Value);
                evaluatedExpression = evaluatedExpression.Substring(match.Index + match.Length);
                match = Regex.Match(evaluatedExpression, EvaluationPatterns.FIRST_PART_DIGIT);
                operandStack.Push(double.Parse(match.Value));
                evaluatedExpression = evaluatedExpression.Substring(match.Index + match.Length);
                var operand2 = operandStack.Pop();
                var operand1 = operandStack.Pop();
                var arithmeticOperator = operatorStack.Pop();
                var operationResult = operationService.Calculate(arithmeticOperator, operand1, operand2);

                if (double.IsInfinity(operationResult))
                {
                    throw new ArithmeticExpressionEvaluationException(Expression, ArithmeticExpressionEvaluationExceptionResource.Infinity_Value);
                }

                operandStack.Push(operationResult);

            }

        }

        if (operandStack.Count == 1)
        {
            return operandStack.Pop();
        }

        Stack<double> operandReversedStack = new Stack<double>(operandStack);
        Stack<string> operatorReversedStack = new Stack<string>(operatorStack);
        while (operandReversedStack.Count > 1)
        {
            var operand1 = operandReversedStack.Pop();
            var operand2 = operandReversedStack.Pop();
            var arithmeticOperator = operatorReversedStack.Pop();
            var operationResult = operationService.Calculate(arithmeticOperator, operand1, operand2);

            if (double.IsInfinity(operationResult))
            {
                throw new ArithmeticExpressionEvaluationException(Expression, ArithmeticExpressionEvaluationExceptionResource.Infinity_Value);
            }

            operandReversedStack.Push(operationResult);
        }


        return operandReversedStack.Pop();
    }


    private void Validate(string expressionString)
    {
        if (expressionString is null || expressionString.Trim().Equals(string.Empty))
        {
            throw new DomainValidationException(ValidationResources.Null_Or_Empty_String);
        }

        if (Regex.Match(expressionString, ValidationPatterns.INVALID_CHARACTERS).Success)
        {
            throw new DomainValidationException(expressionString, ValidationResources.Invalid_Characters);
        }

        if (Regex.Match(expressionString, ValidationPatterns.CONSECUTIVE_OPERATORS).Success)
        {
            throw new DomainValidationException(expressionString, ValidationResources.Consecutive_Operators);
        }

        if (Regex.Match(expressionString, ValidationPatterns.STARTS_OR_ENDS_WITH_NON_DIGITS).Success)
        {
            throw new DomainValidationException(expressionString, ValidationResources.Starts_Or_Ends_With_Operator);
        }

        if (Regex.Match(expressionString, ValidationPatterns.CONTAINS_ONLY_DIGITS).Success)
        {
            throw new DomainValidationException(expressionString, ValidationResources.Contains_Only_Digits);
        }
    }

}
