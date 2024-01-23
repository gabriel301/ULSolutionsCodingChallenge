namespace Shared.Exceptions;
public class EventException : Exception
{
    public EventException() : base() { }

    public EventException(string message) : base(message) { }
}
