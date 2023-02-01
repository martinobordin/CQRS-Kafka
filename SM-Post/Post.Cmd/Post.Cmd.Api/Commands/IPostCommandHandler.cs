namespace Post.Cmd.Api.Commands;

public interface IPostCommandHandler
{
    Task HandleAsync(NewPostCommand command);
    Task HandleAsync(EditMessageCommand command);
    Task HandleAsync(LikePostCommand command);
    Task HandleAsync(AddCommentCommand command);
    Task HandleAsync(EditCommentCommand command);
    Task HandleAsync(RemoveCommentCommand command);
    Task HandleAsync(DeletePostCommand command);
}
