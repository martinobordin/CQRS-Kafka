namespace CQRS.Core.Events;

public abstract class BaseEvent
{
    public Guid Id { get; set; }

    protected BaseEvent(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException($"'{nameof(type)}' cannot be null or whitespace.", nameof(type));
        }

        Type = type;
    }

    public int Version { get; set; }

    public string Type { get; set; }
}
