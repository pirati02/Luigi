using SharedKernel;

namespace Reservation.Domain.Events;

public class CreateDraftReservationEvent: DomainVersionedEvent
{ 
    public Guid InitiatorUserId { get; }
    public DateTime ReservationDateTime { get; }
    public int Row { get; }
    public int Column { get; }
    public Guid SessionId { get; }
    public Guid ReservationId { get; }
    public string InitiatorUserEmail { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public CreateDraftReservationEvent(Guid initiatorUserId, DateTime reservationDateTime, Guid sessionId, int row, int column, Guid reservationId, string email, string firstName, string lastName)
    {
        InitiatorUserId = initiatorUserId;
        SessionId = sessionId;
        Row = row;
        Column = column;
        ReservationId = reservationId;
        ReservationDateTime = reservationDateTime;
        InitiatorUserEmail = email;
        FirstName = firstName;
        LastName = lastName;
    }
}