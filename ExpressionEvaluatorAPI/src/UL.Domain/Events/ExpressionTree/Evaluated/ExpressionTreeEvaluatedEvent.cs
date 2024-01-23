using UL.Shared.Events;

namespace UL.Domain.Events.ExpressionTree.Evaluated;
public class ExpressionTreeEvaluatedEvent : Event<ExpressionTreeEvaluatedEventData>
{
    public ExpressionTreeEvaluatedEvent(ExpressionTreeEvaluatedEventData eventData) : base(eventData)
    {
    }
}
