using Post.Common.Events;
using Post.Query.Domain;
using Post.Query.Domain.Repository;

namespace Post.Query.Infrastructure.Handlers;

public class EventHandler : IEventHandler
{
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;

    public EventHandler(IPostRepository postRepository, ICommentRepository commentRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }

    public async Task On(PostCreatedEvent @event)
    {
        var post = new PostEntity()
        {
            PostId = @event.Id,
            Author = @event.Author,
            DatePosted = @event.DatePosted,
            Message = @event.Message
        };

        await postRepository.CreateAsync(post);
    }

    public async Task On(MessageUpdateEvent @event)
    {
        var post = await postRepository.GetByIdAsync(@event.Id);

        if (post == null)
        {
            return;
        }

        post.Message = @event.Message;
        await postRepository.UpdateAsync(post);
    }

    public async Task On(PostLikedEvent @event)
    {
        var post = await postRepository.GetByIdAsync(@event.Id);

        if (post == null)
        {
            return;
        }

        post.Likes++;

        await postRepository.UpdateAsync(post);
    }

    public async Task On(CommentAddedEvent @event)
    {
        var comment = new CommentEntity()
        {
            PostId = @event.Id,
            CommentId= @event.CommentId,
            Comment = @event.Comment,
            CommentDate= @event.CommentDate,
            Username= @event.Username,
            Edited = false
        };

        await commentRepository.CreateAsync(comment);
    }

    public async Task On(CommentUpdatedEvent @event)
    {
        var comment = await commentRepository.GetByIdAsync(@event.CommentId);

        if (comment == null)
        {
            return;
        }

        comment.Comment = @event.Comment;
        comment.Edited = true;
        comment.CommentDate = @event.EditDate;

        await commentRepository.UpdateAsync(comment);
    }

    public async Task On(CommentRemovedEvent @event)
    {
        await commentRepository.DeleteAsync(@event.CommentId);
    }

    public async Task On(PostRemovedEvent @event)
    {
        await postRepository.DeleteAsync(@event.Id);
    }
}
