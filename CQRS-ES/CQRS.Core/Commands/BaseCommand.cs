namespace CQRS.Core.Commands;

public abstract class BaseCommand
{
    public Guid Id { get; set; }
}
