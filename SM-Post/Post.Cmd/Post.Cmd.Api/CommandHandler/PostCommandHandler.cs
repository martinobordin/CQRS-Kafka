using CQRS.Core.Handlers;
using Post.Cmd.Api.Commands;
using Post.Cmd.Domain;

namespace Post.Cmd.Api.CommandHandler;

public class PostCommandHandler : IPostCommandHandler
{
    private readonly IEventSourcingHandler<PostAggregate> eventSourcingHandler;

    public PostCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    {
        this.eventSourcingHandler = eventSourcingHandler;
    }
    public async Task HandleAsync(NewPostCommand command)
    {
        var aggregate = new PostAggregate(command.Id, command.Username, command.Message);
        await eventSourcingHandler.AppendEventsAsync(aggregate);
    }

    public async Task HandleAsync(EditMessageCommand command)
    {
        var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.EditMessage(command.Message, command.Username);

        await eventSourcingHandler.AppendEventsAsync(aggregate);
    }

    public async Task HandleAsync(LikePostCommand command)
    {
        var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.LikePost();

        await eventSourcingHandler.AppendEventsAsync(aggregate);
    }

    public async Task HandleAsync(AddCommentCommand command)
    {
        var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.AddComment(command.Comment, command.Username);

        await eventSourcingHandler.AppendEventsAsync(aggregate);
    }

    public async Task HandleAsync(EditCommentCommand command)
    {
        var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.EditComment(command.CommentId, command.Comment, command.Username);

        await eventSourcingHandler.AppendEventsAsync(aggregate);
    }

    public async Task HandleAsync(RemoveCommentCommand command)
    {
        var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.RemoveComment(command.CommentId, command.Username);

        await eventSourcingHandler.AppendEventsAsync(aggregate);
    }

    public async Task HandleAsync(DeletePostCommand command)
    {
        var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.DeletePost(command.Username);

        await eventSourcingHandler.AppendEventsAsync(aggregate);
    }

    public async Task HandleAsync(RestoreReadDbCommand command)
    {
        await eventSourcingHandler.RepublishEventsAsync();
    }
}
