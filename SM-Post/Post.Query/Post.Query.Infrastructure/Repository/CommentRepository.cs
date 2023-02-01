using Microsoft.EntityFrameworkCore;
using Post.Query.Domain;
using Post.Query.Domain.Repository;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repository;

public class CommentRepository : ICommentRepository
{
    private readonly DatabaseContextFactory databaseContextFactory;

    public CommentRepository(DatabaseContextFactory databaseContextFactory)
    {
        this.databaseContextFactory = databaseContextFactory;
    }

    public async Task CreateAsync(CommentEntity comment)
    {
        using var context = databaseContextFactory.CreateDbcontext();

        await context.Comments.AddAsync(comment);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        using var context = databaseContextFactory.CreateDbcontext();

        var comment = await GetByIdAsync(id);

        if (comment == null)
        {
            return;
        }

        context.Comments.Remove(comment);
        await context.SaveChangesAsync();
    }

    public async Task<CommentEntity?> GetByIdAsync(Guid id)
    {
        using var context = databaseContextFactory.CreateDbcontext();

        return await context.Comments.FirstOrDefaultAsync(x => x.CommentId == id);
    }

    public async Task UpdateAsync(CommentEntity comment)
    {
        using var context = databaseContextFactory.CreateDbcontext();

        context.Comments.Update(comment);
        await context.SaveChangesAsync();
    }
}
