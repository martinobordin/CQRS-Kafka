using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Query.Api.Queries;
using Post.Query.Domain;

namespace Post.Query.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]

public class PostLookupController : ControllerBase
{
    private readonly ILogger<PostLookupController> logger;
    private readonly IQueryDispatcher<PostEntity> queryDispatcher;

    public PostLookupController(ILogger<PostLookupController> logger, IQueryDispatcher<PostEntity> queryDispatcher)
    {
        this.logger = logger;
        this.queryDispatcher = queryDispatcher;
    }

    [HttpGet]
    public async Task<ActionResult> FindAllPostsAsync()
    {
        var posts = await queryDispatcher.SendAsync(new FindAllPostsQuery());

        if (posts == null || !posts.Any())
        {
            return NoContent();
        }

        return Ok(posts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> FindPostByIdAsync(Guid postId)
    {
        var posts = await queryDispatcher.SendAsync(new FindPostByIdQuery(postId) );

        if (posts == null || !posts.Any())
        {
            return NoContent();
        }

        return Ok(posts);
    }

    [HttpGet("[action]/{id}")]
    public async Task<ActionResult> FindPostByAuthorAsync(string author)
    {
        var posts = await queryDispatcher.SendAsync(new FindPostByAuthorQuery(author));

        if (posts == null || !posts.Any())
        {
            return NoContent();
        }

        return Ok(posts);
    }

    [HttpGet("[action]")]
    public async Task<ActionResult> FindPostsWithCommentsAsync()
    {
        var posts = await queryDispatcher.SendAsync(new FindPostsWithCommentsQuery());

        if (posts == null || !posts.Any())
        {
            return NoContent();
        }

        return Ok(posts);
    }

    [HttpGet("[action]")]
    public async Task<ActionResult> FindPostsWithLikesAsync(int numberOfLikes)
    {
        var posts = await queryDispatcher.SendAsync(new FindPostsWithLikesQuery(numberOfLikes));

        if (posts == null || !posts.Any())
        {
            return NoContent();
        }

        return Ok(posts);
    }
}
