using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reservation.Postgres.Conversions;
using Reservation.Settings.Infrastructure;

namespace Reservation.Postgres;

[Table("Sessions")]
public class ProvidedSession
{
    [Key]
    public Guid Id { get; init; }
    public string Name { get; init; }

    [Column(TypeName = "jsonb")] public SessionSpots Spot { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly Time { get; set; }
}

public record SessionTime(DateOnly Date, TimeOnly Time);

public class SessionConfiguration : IEntityTypeConfiguration<ProvidedSession>
{
    public void Configure(EntityTypeBuilder<ProvidedSession> builder)
    {
        builder.Property(a => a.Date)
            .HasConversion<DateOnlyConverter, DateOnlyComparer>();

        builder.Property(a => a.Time)
            .HasConversion<TimeOnlyConverter, TimeOnlyComparer>();
    }
}