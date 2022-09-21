using Reservation.Domain.ValueObjects;

namespace Reservation.Api.Feature.MakeDraftReservation;

public record SeatModel(int Row, int Column);

public record InitiateUserModel(string FirstName, string LastName, string Email, string Mobile);
 
public record MakeReservationCommand
{
    public Guid SessionId { get; init; }
    public DateTime ReservationTime { get; init; }
    public SeatModel SeatModel { get; init; }
    public InitiateUserModel InitiatorUser { get; init; }
}

