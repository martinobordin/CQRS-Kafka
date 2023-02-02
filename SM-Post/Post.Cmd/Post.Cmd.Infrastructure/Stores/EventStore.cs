using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;

namespace Post.Cmd.Infrastructure.Stores;

public class EventStore : IEventStore
{
    private readonly IEventStoreRepository eventStoreRepository;
    private readonly IEventProducer eventProducer;

    public EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer)
    {
        this.eventStoreRepository = eventStoreRepository;
        this.eventProducer = eventProducer;
    }
    public async Task AppendEventsAsync<T>(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion) where T : AggregateRoot
    {
        var eventStream = await eventStoreRepository.FindByAggregateIdAsync(aggregateId);

        if (expectedVersion != AggregateRoot.StartVersion && eventStream[^1].Version != expectedVersion)
        {
            throw new ConcurrencyException(aggregateId, typeof(T).Name);
        }

        // TODO: Use mongodb transaction

        var version = expectedVersion;
        foreach (var @event in events)
        {
            version++;
            @event.Version = version;
            var eventType = @event.GetType().Name;

            var eventModel = new EventModel
            {
                TimeStamp = DateTime.UtcNow,
                AggregateIdentifier = aggregateId,
                AggregateType = typeof(T).Name,
                EventType = eventType,
                EventData = @event
            };

            await eventStoreRepository.AppendAsync(eventModel);

            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC")!;
            await eventProducer.ProduceAsync(topic, @event);
        }
    }

    public async Task<List<BaseEvent>> GetEventsAsync<T>(Guid aggregateId) where T : AggregateRoot
    {
        var eventStream = await eventStoreRepository.FindByAggregateIdAsync(aggregateId);

        if (eventStream == null || !eventStream.Any())
        {
            throw new AggregateNotFoundException(aggregateId, typeof(T).Name);
        }

        return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList();
    }
}
