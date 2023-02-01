﻿using CQRS.Core.CQRS;

namespace Post.Common.Events;

public class CommentRemovedEvent : BaseEvent
{
    public CommentRemovedEvent()
        : base(nameof(CommentRemovedEvent))
    {
    }

    public Guid CommentId { get; set; }
}