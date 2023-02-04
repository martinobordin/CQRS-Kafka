using Post.Query.Domain;
using Post.Query.Domain.Repository;

namespace Post.Query.Api.Queries;

public class QueryHandler : IQueryHandler
{
    private readonly IPostRepository repository;

    public QueryHandler(IPostRepository repository)
    {
        this.repository = repository;
    }

    public async Task<List<PostEntity>> HandleAsync(FindAllPostsQuery query)
    {
        return await repository.ListAllAsync();
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostByAuthorQuery query)
    {
        return await repository.ListByAuthorAsync(query.Author);
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostByIdQuery query)
    {
        var post = await repository.GetByIdAsync(query.Id);
        return new List<PostEntity>() { post };
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostsWithCommentsQuery query)
    {
        return await repository.ListWithCommentsAsync();
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostsWithLikesQuery query)
    {
        return await repository.ListWithLikesAsync(query.NumberOfLikes);
    }
}