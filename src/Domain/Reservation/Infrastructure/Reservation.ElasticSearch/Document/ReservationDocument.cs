using Reservation.Domain.Model;

namespace Reservation.ElasticSearch.Document;


public record ReservationDocument(
    Guid ReservationId,
    int Row,
    int Column,
    DateTime ReservationDate,
    Guid InitiatorUserId,
    ReservationStatus ReservationStatus,
    Guid SessionId
);