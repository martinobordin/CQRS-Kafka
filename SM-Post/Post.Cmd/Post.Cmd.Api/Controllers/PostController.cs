using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Post.Cmd.Api.Commands;

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
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> EditPostAsync(Guid id, EditMessageCommand command)
    {
        command.Id = id;
        await commandDispatcher.SendAsync(command);
        return Ok();
    }

    [HttpPut("[action]/{id}")]
    public async Task<ActionResult> LikePostAsync(Guid id, LikePostCommand command)
    {
        command.Id = id;
        await commandDispatcher.SendAsync(command);
        return Ok();
    }

    [HttpPost("[action]")]
    public async Task<ActionResult> AddCommentAsync(AddCommentCommand command)
    {
        await commandDispatcher.SendAsync(command);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPut("[action]/{id}")]
    public async Task<ActionResult> EditCommentAsync(Guid id, EditCommentCommand command)
    {
        command.Id = id;
        await commandDispatcher.SendAsync(command);
        return Ok();
    }

    [HttpDelete("[action]/{id}")]
    public async Task<ActionResult> RemoveCommentAsync(Guid id, RemoveCommentCommand command)
    {
        command.Id = id;
        await commandDispatcher.SendAsync(command);
        return Ok();
    }


    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePostAsync(Guid id, DeletePostCommand command)
    {
        command.Id = id;
        await commandDispatcher.SendAsync(command);
        return Ok();
    }

    [HttpPost("[action]")]
    public async Task<ActionResult> RestoreDbAsync()
    {
        var command = new RestoreReadDbCommand();
        await commandDispatcher.SendAsync(command);
        return Ok();
    }
}
