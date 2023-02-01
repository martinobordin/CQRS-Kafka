using CQRS.Core.CQRS;

namespace Post.Common.Events;

public class PostLikedEvent : BaseEvent
{
    public PostLikedEvent() 
        : base(nameof(PostLikedEvent))
    {
    }
}
