namespace UL.Domain.Exceptions.ArithmeticExpression;
public class ArithmeticExpressionEvaluationException : Exception
{
    public string Expression { get; private set; }

    public ArithmeticExpressionEvaluationException() : base() { Expression = string.Empty; }

    public ArithmeticExpressionEvaluationException(string message) : base(message)
    {
        Expression = string.Empty;
    }


    public ArithmeticExpressionEvaluationException(string expression, string message) : base(message)
    {
        Expression = expression;
    }
}

