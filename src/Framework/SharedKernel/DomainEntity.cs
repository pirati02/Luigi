namespace SharedKernel;

public abstract class DomainEntity
{
    public Guid Id { get; protected set; }

    public DomainEntity()
    {
        Id = Guid.NewGuid();
    }

    private readonly List<IDomainEvent> _events = new();
    protected long Version { get; set; } = -1;

    protected abstract void ApplyEvent(object @event);

    public void Apply(IDomainEvent @event)
    {
        ApplyEvent(@event);

        _events.Add(@event);
    }

    public void Load(long version, IReadOnlyList<DomainVersionedEvent?> history)
    {
        Version = version;
        foreach (var e in history)
        {
            ApplyEvent(e);
        }
    }

    public IDomainEvent[] GetChanges() => _events.ToArray();
}