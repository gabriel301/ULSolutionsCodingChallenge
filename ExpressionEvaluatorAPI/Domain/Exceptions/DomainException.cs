namespace Domain.Exceptions;
public class DomainException : Exception
{
    
    public string Expression { get; private set; }
    
    public DomainException() : base() { Expression = string.Empty; }

    public DomainException(string message) : base(message) 
    {
        Expression = string.Empty;
    }


    public DomainException(string expression, string message) : base(message)
    {
        Expression = expression;
    }

}
