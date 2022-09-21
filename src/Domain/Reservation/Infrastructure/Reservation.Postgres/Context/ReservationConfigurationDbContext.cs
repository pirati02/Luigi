using Microsoft.EntityFrameworkCore;

namespace Reservation.Postgres;

public class ReservationConfigurationDbContext : DbContext
{
    public DbSet<ProvidedSession> Sessions { get; set; }

    public ReservationConfigurationDbContext(DbContextOptions<ReservationConfigurationDbContext> options) :
        base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReservationConfigurationDbContext).Assembly);
    }
}