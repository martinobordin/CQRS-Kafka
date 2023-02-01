using CQRS.Core.CQRS;

namespace Post.Common.Events;

public class PostRemovedEvent : BaseEvent
{
    public PostRemovedEvent()
        : base(nameof(PostRemovedEvent))
    {
    }
}