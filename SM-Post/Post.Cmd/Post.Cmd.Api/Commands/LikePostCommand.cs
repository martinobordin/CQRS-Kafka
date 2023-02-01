using CQRS.Core.CQRS;

namespace Post.Cmd.Api.Commands;

public class LikePostCommand : BaseCommand
{
    public string Author { get; set; }

    public string Message { get; set; }
}
