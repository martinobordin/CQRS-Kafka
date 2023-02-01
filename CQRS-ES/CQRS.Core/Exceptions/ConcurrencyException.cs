namespace CQRS.Core.Exceptions;

public class ConcurrencyException : Exception
{
    public Guid AggregateId { get; }
    public string AggregateType { get; }

    public ConcurrencyException(Guid aggregateId, string aggregateType)
        : base($"Version not expected for aggregate  of type {aggregateType} with id {aggregateId}")
    {
        AggregateId = aggregateId;
        AggregateType = aggregateType;
    }
}
