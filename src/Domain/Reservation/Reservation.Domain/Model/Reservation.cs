using Reservation.Domain.Events;
using Reservation.Domain.ValueObjects;
using SharedKernel;

namespace Reservation.Domain.Model;

public class Reservation : DomainEntity, IAggregateRoot
{
    private Reservation(InitiatorUser initiatorUser, ReservationTime reservationTime, Seat seat, Session session)
    {
        InitiatorUser = initiatorUser;
        ReservationTime = reservationTime;
        Seat = seat;
        Session = session;
    }

    public Reservation()
    {
        
    }

    public ReservationStatus ReservationStatus { get; private set; }
    public InitiatorUser InitiatorUser { get; private set; }
    public ReservationTime ReservationTime { get; private set; }
    public Seat Seat { get; }
    public Session Session { get; private set; }

    protected override void ApplyEvent(object @event)
    {
        switch (@event)
        {
            case CreateDraftReservationEvent x:
                OnDraftReservationCreated(x);
                break;
            case MakeReservationActive:
                OnActivate();
                break;
        }
    }

    private void OnActivate()
    {
        ReservationStatus = ReservationStatus.ActiveUnpaid;
    }

    public static Reservation CreateDraft(InitiatorUser initiatorUser, ReservationTime time, Seat seat, Session session)
    {
        var reservation = new Reservation(initiatorUser, time, seat, session);
        reservation.Apply(new CreateDraftReservationEvent(initiatorUser.Id, time.AsDateTime(), session.Id, seat.Row, seat.Column, Guid.NewGuid())
        {
            Version = reservation.Version + 1
        });
        return reservation;
    }

    private void OnDraftReservationCreated(CreateDraftReservationEvent @event)
    {
        Id = @event.ReservationId;
        Version = @event.Version;
        InitiatorUser = InitiatorUser.From(@event.InitiatorUserId);
        ReservationTime = ReservationTime.From(@event.ReservationDateTime);
        ReservationStatus = ReservationStatus.Draft;
        Session = Session.From(@event.SessionId);
    }

    public void MakeActive()
    {
        Apply(new MakeReservationActive());        
    }
}

public enum ReservationStatus
{
    Draft,
    ActiveUnpaid,
    ActiveAsPerPaid
}