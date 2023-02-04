using CQRS.Core.Domain;

namespace CQRS.Core.Handlers;

public interface IEventSourcingHandler<T>
{
    Task AppendEventsAsync(AggregateRoot aggregateRoot);
    Task<T> GetByIdAsync(Guid aggregateId);
    Task RepublishEventsAsync();
}
