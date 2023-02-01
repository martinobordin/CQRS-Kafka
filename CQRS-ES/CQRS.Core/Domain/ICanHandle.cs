using CQRS.Core.Events;

namespace CQRS.Core.Domain;

public interface ICanHandle<T> where T : BaseEvent
{
    void Apply(T @event);
}
