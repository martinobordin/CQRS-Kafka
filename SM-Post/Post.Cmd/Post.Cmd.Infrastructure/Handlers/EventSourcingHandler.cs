using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain;

namespace Post.Cmd.Infrastructure.Handlers;

public class PostEventSourcingHandler : IEventSourcingHandler<PostAggregate>
{
    private readonly IEventStore eventStore;
    private readonly IEventProducer eventProducer;

    public PostEventSourcingHandler(IEventStore eventStore, IEventProducer eventProducer)
    {
        this.eventStore = eventStore;
        this.eventProducer = eventProducer;
    }
    public async Task AppendEventsAsync(AggregateRoot aggregateRoot)
    {
        await this.eventStore.AppendEventsAsync<PostAggregate>(aggregateRoot.Id, aggregateRoot.GetUncommitedChanges(), aggregateRoot.Version);
        aggregateRoot.MarkChangesAsCommitted();
    }
    public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
    {
        var aggregate = new PostAggregate();
        var events = await eventStore.GetEventsAsync<PostAggregate>(aggregateId);

        if (events == null || !events.Any())
        {
            return aggregate;
        }

        aggregate.ReplayEvents(events);

        var latestVersion = events.Select(x => x.Version).Max();
        aggregate.Version = latestVersion;

        return aggregate;
    }

    public async Task RepublishEventsAsync()
    {
        var aggregateIds = await eventStore.GetAggregateIdsAsyc();
        if (aggregateIds == null || !aggregateIds.Any()) 
        { 
            return; 
        }

        foreach (var aggregateId in aggregateIds)
        {
            var aggregate = await GetByIdAsync(aggregateId);

            if (aggregate == null || !aggregate.Active)
            {
                continue;
            }

            var events = await eventStore.GetEventsAsync<PostAggregate>(aggregateId);

            foreach (var @event in events)
            {
                var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC")!;
                await this.eventProducer.ProduceAsync(topic, @event);
            }
        }
    }
}
