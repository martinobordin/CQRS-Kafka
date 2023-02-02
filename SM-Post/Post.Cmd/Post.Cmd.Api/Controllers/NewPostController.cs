using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.Dtos;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class NewPostController : ControllerBase
{
    private readonly ILogger<NewPostController> logger;
    private readonly ICommandDispatcher commandDispatcher;

    public NewPostController(ILogger<NewPostController> logger, ICommandDispatcher commandDispatcher)
	{
        this.logger = logger;
        this.commandDispatcher = commandDispatcher;
    }

    [HttpPost]
    public async Task<ActionResult> NewPostAsync(NewPostCommand command)
    {
        try
        {
            var id = Guid.NewGuid();
            command.Id = id;

            await commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status202Accepted, new NewPostResponse()
            {
                Message = "New post creation request completed successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Client made a bad request");
            return StatusCode(StatusCodes.Status400BadRequest, new NewPostResponse()
            {
                Message = ex.Message
            });
        }
        catch(Exception ex)
        {
            logger.LogWarning(ex, "Error while processing request to create a new post");
            return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse()
            {
                Message = "Error while processing request to create a new post"
            });
        }
        
    }
}
