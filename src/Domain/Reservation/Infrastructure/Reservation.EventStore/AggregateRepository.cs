using Marten;
using Marten.Storage;
using SharedKernel;

namespace Reservation.EventStore;

public class AggregateRepository
{
    private readonly IDocumentStore _eventStore;

    public AggregateRepository(IDocumentStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task SaveAsync<T>(T aggregate, CancellationToken ct)
        where T : DomainEntity, new()
    {
        await using var session = _eventStore.OpenSession();
        await using var querySession = _eventStore.QuerySession();
        var domainEvents = aggregate.GetChanges()
            .Select(a => (object)a)
            .ToArray();

        if (!domainEvents.Any())
        {
            return;
        }

        var streamState = await session.Events.FetchStreamStateAsync(aggregate.Id, ct);
        if (streamState is null)
        {
            session.Events.StartStream(id: aggregate.Id, events: domainEvents);
        }
        else
        {
            session.Events.Append(aggregate.Id, domainEvents);
        }

        await session.SaveChangesAsync(ct);
    }

    public async Task<T> LoadAsync<T>(Guid aggregateId) where T : DomainEntity, new()
    {
        if (aggregateId == Guid.Empty)
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(aggregateId));

        await using var session = _eventStore.OpenSession();
        var aggregate = new T();

        var page = await session.Events.FetchStreamAsync(aggregateId);

        if (page.Count > 0)
        {
            aggregate.Load(
                page.Last().Version,
                page.Select(@event =>
                {
                    var evt = @event.Data as DomainVersionedEvent;
                    if (evt is not null)
                    {
                        evt.Version = @event.Version;
                    }
                    return evt;
                }).ToArray()
            );
        }

        return aggregate;
    }
}