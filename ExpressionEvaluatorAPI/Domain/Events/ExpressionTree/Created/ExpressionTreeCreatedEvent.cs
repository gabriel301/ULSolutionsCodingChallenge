using Shared.Events;

namespace Domain.Events.ExpressionTree.Created;
public class ExpressionTreeCreatedEvent : Event<ExpressionTreeCreatedEventData>
{
    public ExpressionTreeCreatedEvent(ExpressionTreeCreatedEventData eventData) : base(eventData)
    {
    }
}
