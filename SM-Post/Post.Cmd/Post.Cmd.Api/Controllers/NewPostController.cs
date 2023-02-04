using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.Dtos;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PostController : ControllerBase
{
    private readonly ILogger<PostController> logger;
    private readonly ICommandDispatcher commandDispatcher;

    public PostController(ILogger<PostController> logger, ICommandDispatcher commandDispatcher)
    {
        this.logger = logger;
        this.commandDispatcher = commandDispatcher;
    }

    [HttpPost]
    public async Task<ActionResult> NewPostAsync(NewPostCommand command)
    {
        await commandDispatcher.SendAsync(command);
        return StatusCode(StatusCodes.Status202Accepted, new NewPostResponse()
        {
            Message = "New post creation request completed successfully"
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> EditPostAsync(Guid id, EditMessageCommand command)
    {
        command.Id = id;
        await commandDispatcher.SendAsync(command);
        return Ok(new BaseResponse()
        {
            Message = "Edit message request completed successfully"
        });
    }
}
