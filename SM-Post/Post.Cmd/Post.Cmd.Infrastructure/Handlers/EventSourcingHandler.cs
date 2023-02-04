using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain;

namespace Post.Cmd.Infrastructure.Handlers;

public class PostEventSourcingHandler : IEventSourcingHandler<PostAggregate>
{
    private readonly IEventStore eventStore;

    public PostEventSourcingHandler(IEventStore eventStore)
    {
        this.eventStore = eventStore;
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
}
