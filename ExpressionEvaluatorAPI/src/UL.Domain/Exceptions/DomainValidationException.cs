namespace UL.Domain.Exceptions;
public class DomainValidationException : Exception
{

    public string Expression { get; private set; }

    public DomainValidationException() : base() { Expression = string.Empty; }

    public DomainValidationException(string message) : base(message)
    {
        Expression = string.Empty;
    }


    public DomainValidationException(string expression, string message) : base(message)
    {
        Expression = expression;
    }

}
