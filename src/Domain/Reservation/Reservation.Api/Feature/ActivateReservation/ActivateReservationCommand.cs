namespace Reservation.Api.Feature.ActivateReservation;

public class ActivateReservationCommand
{
    public Guid SessionId { get; init; }
    public Guid InitiatorUserId { get; init; }
    public Guid ReservationId { get; init; }
}
