using Reservation.Domain.ValueObjects;
using SharedKernel;

namespace Reservation.Domain.Events;

public class ReserveSeatEvent: DomainVersionedEvent
{
    public Seat Seat { get; }

    public ReserveSeatEvent(Seat seat)
    {
        Seat = seat;
    }
}