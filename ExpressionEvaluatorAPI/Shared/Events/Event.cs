using Shared.Exceptions;
using Shared.Resources;

namespace Shared.Events;
public abstract class Event<TEventData> : IEvent
{
    public DateTimeOffset LocalEventDateTime { get; private set; }

    public DateTimeOffset UTCEventDateTime { get; private set; }

    public TEventData EventData { get; private set; }

    public Event(TEventData eventData)
    {

        if (eventData == null)
        {
            throw new EventException(string.Format(EventResources.Null_Or_Default_Value, nameof(eventData)));
        }

        LocalEventDateTime = DateTimeOffset.Now;
        UTCEventDateTime = DateTimeOffset.UtcNow; ;
        EventData = eventData;
    }

    public override string ToString()
    {
        return string.Format(EventResources.Event_String, this.GetType().Name, nameof(UTCEventDateTime), UTCEventDateTime.ToString(), nameof(LocalEventDateTime), LocalEventDateTime.ToString(), EventData!.ToString());
    }

}
