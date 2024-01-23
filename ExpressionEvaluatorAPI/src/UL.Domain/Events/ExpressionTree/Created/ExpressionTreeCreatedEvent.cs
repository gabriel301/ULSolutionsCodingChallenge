using UL.Shared.Events;

namespace UL.Domain.Events.ExpressionTree.Created;
public class ExpressionTreeCreatedEvent : Event<ExpressionTreeCreatedEventData>
{
    public ExpressionTreeCreatedEvent(ExpressionTreeCreatedEventData eventData) : base(eventData)
    {
    }
}
