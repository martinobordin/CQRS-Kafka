using CQRS.Core.CQRS;
using CQRS.Core.Infrastructure;

namespace Post.Query.Infrastructure;
public class CommandDispatcher : ICommandDispatcher
{
    private readonly Dictionary<Type, Func<BaseCommand, Task>> handlers = new();

    public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
    {
        if (handlers.ContainsKey(typeof(T)))
        {
            throw new InvalidOperationException("You cannot register an handler twice");
        }

        handlers.Add(typeof(T), x => handler((T)x));
    }

    public async Task SendAsync(BaseCommand command)
    {
        if (handlers.TryGetValue(command.GetType(), out Func<BaseCommand, Task>? handler))
        {
            await handler(command);
        }

        throw new InvalidOperationException("No command handler was registered");
    }
}
