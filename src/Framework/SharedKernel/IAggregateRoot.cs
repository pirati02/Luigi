namespace SharedKernel;

public interface IAggregateRoot
{
    public void Apply(IDomainEvent @event);

    public void Load(long version, IReadOnlyList<DomainVersionedEvent> history);

    public IDomainEvent[] GetChanges();
}