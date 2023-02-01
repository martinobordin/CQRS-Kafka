using CQRS.Core.Events;

namespace CQRS.Core.Domain;

public abstract class AggregateRoot
{
    public const int StartVersion = -1;
    public Guid Id { get; protected  set; }

    private readonly List<BaseEvent> changes = new();

    public int Version { get; set; } = StartVersion;

    public IReadOnlyCollection<BaseEvent> GetUncommitedChanges()
    {
        return changes.AsReadOnly();
    }

    public void MarkChangesAsCommitted()
    {
        changes.Clear();
    }

    private void ApplyChange(BaseEvent @event, bool isNew)
    {
        var eventType = @event.GetType();
        var concreteAggregate = this.GetType();
        var concreteApplyMethod = concreteAggregate
            .GetMethod("Apply", new Type[] { eventType });

        if (concreteApplyMethod == null)
        {
            throw new InvalidOperationException($"The Apply method for the event {eventType.Name} was not found in the aggregate {concreteAggregate.Name}");
        }

        concreteApplyMethod.Invoke(this, new object[] { @event });

        if (!isNew )
        {
            changes.Add(@event);
        }
    }

    protected void RaiseEvent(BaseEvent @event)
    {
        ApplyChange(@event, true);
    }

    public void ReplayEvents(IEnumerable<BaseEvent> events)
    {
        foreach (var @event in events)
        {
            ApplyChange(@event, false);
        }
    }
}
