using CQRS.Core.Domain;
using CQRS.Core.Events;

namespace CQRS.Core.Infrastructure;

public interface IEventStore
{
    Task AppendEventsAsync<T>(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion) where T : AggregateRoot;
    Task<List<BaseEvent>> GetEventsAsync<T>(Guid aggregateId) where T : AggregateRoot;

    Task<List<Guid>> GetAggregateIdsAsyc();
}