using CQRS.Core.CQRS;

namespace Post.Common.Events;

public class MessageUpdateEvent : BaseEvent
{
    public MessageUpdateEvent() 
        : base(nameof(MessageUpdateEvent))
    {
    }

    public string Message { get; set; }
}
