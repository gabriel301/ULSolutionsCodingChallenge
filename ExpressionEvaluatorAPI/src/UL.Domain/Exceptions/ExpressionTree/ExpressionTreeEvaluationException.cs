namespace UL.Domain.Exceptions.ExpressionTree;
public class ExpressionTreeEvaluationException : Exception
{
    public string Expression { get; private set; }

    public ExpressionTreeEvaluationException() : base() { Expression = string.Empty; }

    public ExpressionTreeEvaluationException(string message) : base(message)
    {
        Expression = string.Empty;
    }


    public ExpressionTreeEvaluationException(string expression, string message) : base(message)
    {
        Expression = expression;
    }
}
