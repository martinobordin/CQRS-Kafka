namespace CQRS.Core.Exceptions;

public class AggregateNotFoundException : Exception
{
    public Guid AggregateId { get; }
    public string AggregateType { get; }

    public AggregateNotFoundException(Guid aggregateId, string aggregateType) 
        : base($"Aggregate of type {aggregateType} with id {aggregateId} not found")
    {
        AggregateId = aggregateId;
        AggregateType = aggregateType;
    }
}
