using Marten.Storage;
using Microsoft.EntityFrameworkCore;
using Reservation.EventStore;
using Reservation.Postgres;

namespace Reservation.Api.Infrastructure;

public static class DatabaseCreationChecker
{
    public static void EnsureSessionStorageCreated(this WebApplication webApplication)
    {
        using var scope = webApplication.Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<ReservationConfigurationDbContext>();
        dbContext.Database.Migrate();
    }
    
    public static void EnsureEventStorageCreated(this WebApplication webApplication)
    {
        using var scope = webApplication.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EventStoreDb>();
        dbContext.Database.Migrate();
    }
}