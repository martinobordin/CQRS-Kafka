using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain;
public class PostAggregate : AggregateRoot,
    ICanHandle<PostCreatedEvent>,
    ICanHandle<MessageUpdateEvent>,
    ICanHandle<PostLikedEvent>,
    ICanHandle<CommentAddedEvent>,
    ICanHandle<CommentUpdatedEvent>,
    ICanHandle<CommentRemovedEvent>,
    ICanHandle<PostRemovedEvent>
{
    public string Author { get; private set; }
    private readonly Dictionary<Guid, Tuple<string, string>> comments = new();

    public bool Active { get; private set; }

    public PostAggregate(Guid id, string author, string message)
    {
        RaiseEvent(new PostCreatedEvent
        {
            Id = id,
            Author = author,
            Message = message,
            DatePosted = DateTime.UtcNow
        });
    }

    public PostAggregate()
    {
    }

    public void EditMessage(string message, string username)
    {
        if (!Active)
        {
            throw new InvalidOperationException("You cannot edit the message of an inactive post");
        }

        if (!Author.Equals(username, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new InvalidOperationException("You cannot edit a post created by another author");
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new InvalidOperationException($"The {nameof(message)} cannot be null or empty");
        }

        RaiseEvent(new MessageUpdateEvent
        {
            Id = Id,
            Message = message
        });
    }

    public void LikePost()
    {
        if (!Active)
        {
            throw new InvalidOperationException("You cannot like an inactive post");
        }

        RaiseEvent(new PostLikedEvent { Id = Id });
    }

    public void AddComment(string comment, string username)
    {
        if (!Active)
        {
            throw new InvalidOperationException("You cannot add a comment of an inactive post");
        }

        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new InvalidOperationException($"The {nameof(comment)} cannot be null or empty");
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            throw new InvalidOperationException($"The {nameof(username)} cannot be null or empty");
        }

        RaiseEvent(new CommentAddedEvent
        {
            Id = Id,
            CommentId = Guid.NewGuid(),
            Comment = comment,
            Username = username,
            CommentDate = DateTime.UtcNow
        });
    }

    public void EditComment(Guid commentId, string comment, string username)
    {
        if (!Active)
        {
            throw new InvalidOperationException("You cannot edit a comment of an inactive post");
        }

        if (!comments.ContainsKey(commentId))
        {
            throw new InvalidOperationException("You cannot edit a comment that doesn't exist");
        }

        if (!comments[commentId].Item2.Equals(username, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new InvalidOperationException("You cannot edit a comment created by another user");
        }

        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new InvalidOperationException($"The {nameof(comment)} cannot be null or empty");
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            throw new InvalidOperationException($"The {nameof(username)} cannot be null or empty");
        }

        RaiseEvent(new CommentUpdatedEvent
        {
            Id = Id,
            CommentId = commentId,
            Comment = comment,
            Username = username,
            EditDate = DateTime.UtcNow
        });
    }

    public void RemoveComment(Guid commentId, string username)
    {
        if (!Active)
        {
            throw new InvalidOperationException("You cannot edit a comment of an inactive post");
        }

        if (!comments.ContainsKey(commentId))
        {
            throw new InvalidOperationException("You cannot remove a comment that doesn't exist");
        }

        if (!comments[commentId].Item2.Equals(username, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new InvalidOperationException("You cannot remove a comment created by another user");
        }

        RaiseEvent(new CommentRemovedEvent()
        {
            Id = Id,
            CommentId = commentId
        });
    }

    public void DeletePost(string username)
    {
        if (!Active)
        {
            throw new InvalidOperationException("The post has already been deleted");
        }

        if (!Author.Equals(username, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new InvalidOperationException("You cannot delete a post created by another author");
        }

        RaiseEvent(new PostRemovedEvent
        {
            Id = Id
        });
    }

    public void Apply(PostCreatedEvent @event)
    {
        Id = @event.Id;
        Active = true;
        Author = @event.Author;
    }

    public void Apply(MessageUpdateEvent @event)
    {
        Id = @event.Id;
    }

    public void Apply(PostLikedEvent @event)
    {
        Id = @event.Id;
    }

    public void Apply(CommentAddedEvent @event)
    {
        Id = @event.Id;
        comments.Add(@event.CommentId, new Tuple<string, string>(@event.Comment, @event.Username));
    }

    public void Apply(CommentUpdatedEvent @event)
    {
        Id = @event.Id;
        comments[@event.CommentId] = new Tuple<string, string>(@event.Comment, @event.Username);
    }

    public void Apply(CommentRemovedEvent @event)
    {
        Id = @event.Id;
        comments.Remove(@event.CommentId);
    }

    public void Apply(PostRemovedEvent @event)
    {
        Id = @event.Id;
        Active = false;
    }
}
