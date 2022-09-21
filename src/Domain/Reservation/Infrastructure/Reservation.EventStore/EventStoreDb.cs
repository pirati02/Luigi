using Microsoft.EntityFrameworkCore;

namespace Reservation.EventStore;

public class EventStoreDb : DbContext
{
    public EventStoreDb(DbContextOptions<EventStoreDb> options) : base(options)
    {
    }
}