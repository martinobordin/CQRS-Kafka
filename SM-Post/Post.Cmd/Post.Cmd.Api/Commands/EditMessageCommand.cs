using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands;

public class EditMessageCommand : BaseCommand
{
    public string Username { get; set; }

    public string Message { get; set; }
}
