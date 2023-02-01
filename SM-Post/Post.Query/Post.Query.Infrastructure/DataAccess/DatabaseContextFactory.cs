using Microsoft.EntityFrameworkCore;

namespace Post.Query.Infrastructure.DataAccess;

public class DatabaseContextFactory
{
    private readonly Action<DbContextOptionsBuilder> configureDbContext;

    public DatabaseContextFactory(Action<DbContextOptionsBuilder> configureDbContext)
    {
        this.configureDbContext = configureDbContext;
    }

    public DatabaseContext CreateDbcontext()
    {
        DbContextOptionsBuilder<DatabaseContext> options = new DbContextOptionsBuilder<DatabaseContext>();
        configureDbContext(options);

        return new DatabaseContext(options.Options);
    }
}