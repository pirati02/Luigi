using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Reservation.Settings.Infrastructure;

public class ReservationConfigurationDbContextFactory : IDesignTimeDbContextFactory<ReservationConfigurationDbContext>
{
    public ReservationConfigurationDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
            .AddJsonFile("Infrastructure/ConnectionStrings/postgres.json", optional: false, reloadOnChange: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ReservationConfigurationDbContext>();
 
        optionsBuilder.UseNpgsql(config.GetConnectionString("ReservationDb"));

        return new ReservationConfigurationDbContext(optionsBuilder.Options);
    }
}