namespace SharedKernel;

public interface IDomainEvent
{
    public long Version { get; }
    public Guid Id { get; }
}

public abstract class DomainVersionedEvent: IDomainEvent
{ 
    public long Version { get; set; }
    public Guid Id { get; set; }

    public DomainVersionedEvent()
    {
        Id = Guid.NewGuid();
    }
}