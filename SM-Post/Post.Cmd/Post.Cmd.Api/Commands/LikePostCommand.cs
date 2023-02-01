using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands;

public class LikePostCommand : BaseCommand
{
    public string Username { get; set; }

    public string Message { get; set; }
}
