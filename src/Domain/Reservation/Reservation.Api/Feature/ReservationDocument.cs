using Reservation.Domain.Model;

namespace Reservation.Api.Feature;


public record ReservationDocument(
    Guid ReservationId,
    int Row,
    int Column,
    DateTime ReservationDate,
    Guid InitiatorUserId,
    ReservationStatus ReservationStatus,
    Guid SessionId
);