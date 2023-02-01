using CQRS.Core.CQRS;

namespace Post.Cmd.Api.Commands;

public class EditMessageCommand : BaseCommand
{
    public string Author { get; set; }

    public string Message { get; set; }
}
