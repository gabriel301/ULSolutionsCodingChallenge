using UL.Shared.Events;

namespace UL.Domain.Events.ArithmeticExpression.Evaluated;
public class ArithmeticExpressionEvaluatedEvent : Event<ArithmeticExpressionEvaluatedEventData>
{
    public ArithmeticExpressionEvaluatedEvent(ArithmeticExpressionEvaluatedEventData eventData) : base(eventData)
    {
    }
}
