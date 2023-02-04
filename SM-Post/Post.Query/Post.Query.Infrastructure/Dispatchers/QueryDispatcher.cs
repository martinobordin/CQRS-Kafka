using CQRS.Core.Infrastructure;
using CQRS.Core.Queries;
using Post.Query.Domain;

namespace Post.Query.Infrastructure.Dispatchers;

public class QueryDispatcher : IQueryDispatcher<PostEntity>
{
    private readonly Dictionary<Type, Func<BaseQuery, Task<List<PostEntity>>>> handlers = new();

    public void RegisterHandler<TQuery>(Func<TQuery, Task<List<PostEntity>>> handler) where TQuery : BaseQuery
    {
        if (handlers.ContainsKey(typeof(TQuery)))
        {
            throw new InvalidOperationException("You cannot register an handler twice");
        }

        handlers.Add(typeof(TQuery), x => handler((TQuery)x));
    }

    public async Task<List<PostEntity>> SendAsync(BaseQuery baseQuery)
    {
        if (handlers.TryGetValue(baseQuery.GetType(), out var handler))
        {
            return await handler(baseQuery);
        }

        throw new InvalidOperationException("No query handler was registered");
    }
}
