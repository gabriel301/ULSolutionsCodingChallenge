using UL.Shared.Events;

namespace UL.Domain.Events.ArithmeticExpression.Created;
public class ArithmeticExpressionCreatedEvent : Event<ArithmeticExpressionCreatedEventData>
{
    public ArithmeticExpressionCreatedEvent(ArithmeticExpressionCreatedEventData eventData) : base(eventData)
    {
    }
}
