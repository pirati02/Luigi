using Marten.Events.Aggregation;
using Reservation.Domain.Events;

namespace Reservation.Infrastructure.Projection;

public class ReservationProjection: SingleStreamAggregation<Domain.Model.Reservation>
{
    public ReservationProjection()
    {
        ProjectEvent<CreateDraftReservationEvent>((reservation, @event) =>
        {
            reservation.Apply(@event);
        });
    }
}