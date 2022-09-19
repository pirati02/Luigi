namespace Reservation.Api.Feature.MakeDraftReservation;

public record SeatModel(int Row, int Column);

public record MakeReservationCommand
{
    public Guid SessionId { get; init; }
    public DateTime ReservationTime { get; init; }
    public SeatModel SeatModel { get; init; }
    public Guid InitiatorUserId { get; init; }
}
