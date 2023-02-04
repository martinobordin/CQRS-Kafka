using Microsoft.EntityFrameworkCore;
using Post.Query.Domain;
using Post.Query.Domain.Repository;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repository;

public class PostRepository : IPostRepository
{
    private readonly DatabaseContextFactory databaseContextFactory;

    public PostRepository(DatabaseContextFactory databaseContextFactory)
    {
        this.databaseContextFactory = databaseContextFactory;
    }

    public async Task CreateAsync(PostEntity post)
    {
        using var context = databaseContextFactory.CreateDbcontext();

        await context.Posts.AddAsync(post);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        using var context = databaseContextFactory.CreateDbcontext();

        var post = await GetByIdAsync(id);

        if (post == null)
        {
            return;
        }

        context.Posts.Remove(post);
        await context.SaveChangesAsync();
    }

    public async Task<PostEntity?> GetByIdAsync(Guid id)
    {
        using var context = databaseContextFactory.CreateDbcontext();

        return await context.Posts.Include(x => x.Comments).FirstOrDefaultAsync(x => x.PostId == id);
    }

    public async Task<List<PostEntity>> ListAllAsync()
    {
        using var context = databaseContextFactory.CreateDbcontext();

        return await context.Posts.AsNoTrackingWithIdentityResolution()
            .Include(x => x.Comments).AsNoTrackingWithIdentityResolution()
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListByAuthorAsync(string author)
    {
        using var context = databaseContextFactory.CreateDbcontext();

        return await context.Posts.AsNoTrackingWithIdentityResolution()
            .Include(x => x.Comments).AsNoTrackingWithIdentityResolution()
            .Where(x => x.Author.Contains(author))
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithCommentsAsync()
    {
        using var context = databaseContextFactory.CreateDbcontext();

        return await context.Posts.AsNoTrackingWithIdentityResolution()
            .Include(x => x.Comments).AsNoTrackingWithIdentityResolution()
            .Where(x => x.Comments != null && x.Comments.Any())
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes)
    {
        using var context = databaseContextFactory.CreateDbcontext();

        return await context.Posts.AsNoTrackingWithIdentityResolution()
            .Include(x => x.Comments).AsNoTrackingWithIdentityResolution()
            .Where(x => x.Likes >= numberOfLikes)
            .ToListAsync();
    }

    public async Task UpdateAsync(PostEntity post)
    {
        using var context = databaseContextFactory.CreateDbcontext();

        context.Posts.Update(post);
        await context.SaveChangesAsync();
    }
}
