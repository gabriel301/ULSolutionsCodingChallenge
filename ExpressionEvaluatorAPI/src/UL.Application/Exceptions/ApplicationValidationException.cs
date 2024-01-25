namespace UL.Application.Exceptions;
public class ApplicationValidationException : Exception
{
    public IEnumerable<ValidationError> Errors { get; }

    public ApplicationValidationException(IEnumerable<ValidationError> errors)
    {
        Errors = errors;
    }
}
