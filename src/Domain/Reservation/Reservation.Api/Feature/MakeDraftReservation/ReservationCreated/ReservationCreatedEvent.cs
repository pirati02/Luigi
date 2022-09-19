using FastEndpoints;
using Nest;
using Reservation.Domain.Model;
using Reservation.Domain.ValueObjects;
using Reservation.ElasticSearch;
using Reservation.ElasticSearch.Configure;
using Reservation.ElasticSearch.Document;
using SharedKernel;

namespace Reservation.Api.Feature.MakeDraftReservation.ReservationCreated;

public record ReservationCreatedEvent()
{
    public Guid ReservationId { get; init; }
    public Seat Seat { get; init; }
    public ReservationTime ReservationTime { get; init; }
    public InitiatorUser InitiatorUserId { get; init; }
    public ReservationStatus ReservationStatus { get; init; }
    public Session Session { get; set; }

    public ReservationCreatedEvent(
        Guid reservationId,
        Seat seat,
        ReservationTime reservationTime,
        InitiatorUser initiatorUserId,
        ReservationStatus reservationStatus,
        Session session
    )
        : this()
    {
        Seat = seat;
        ReservationTime = reservationTime;
        InitiatorUserId = initiatorUserId;
        ReservationStatus = reservationStatus;
        Session = session;
        ReservationId = reservationId;
    }
}

public class ReservationCreatedEventHandler : FastEventHandler<ReservationCreatedEvent>
{
    public override async Task<Task> HandleAsync(ReservationCreatedEvent eventModel, CancellationToken ct)
    {
        var elasticClient = Resolve<IElasticClient>();

        var document = new ReservationDocument(
            eventModel.ReservationId,
            eventModel.Seat.Row,
            eventModel.Seat.Column,
            eventModel.ReservationTime.Date.ToDateTime(eventModel.ReservationTime.Time),
            eventModel.InitiatorUserId.Id,
            eventModel.ReservationStatus,
            eventModel.Session.Id
        );
        var result = await elasticClient.IndexAsync(
            document,
            a => a.Index(ElasticConfiguration.IndexConfig.ReservationEventIndex),
            ct
        );

        return result.Result == Result.Created
            ? Task.CompletedTask
            : Task.FromException(new DomainException("reservation_document_creation_failed"));
    }
}
