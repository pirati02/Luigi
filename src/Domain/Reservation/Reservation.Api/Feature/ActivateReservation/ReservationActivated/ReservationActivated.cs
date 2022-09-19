using Elasticsearch.Net;
using FastEndpoints;
using Nest;
using Reservation.Api.Infrastructure.ElasticSearch;
using Reservation.Api.Service;
using Reservation.Domain.Model;
using Reservation.Domain.ValueObjects;
using SharedKernel;

namespace Reservation.Api.Feature.ActivateReservation.ReservationActivated;

public record ReservationActivatedEvent()
{
    public Guid ReservationId { get; init; }
    public Guid InitiatorUserId { get; init; }
    public Session Session { get; set; }

    public ReservationActivatedEvent(
        Guid reservationId,
        Session session,
        Guid initiatorUserId
    )
        : this()
    {
        Session = session;
        InitiatorUserId = initiatorUserId;
        ReservationId = reservationId;
    }
}

public class ReservationActivatedEventHandler : FastEventHandler<ReservationActivatedEvent>
{
    public override async Task<Task> HandleAsync(ReservationActivatedEvent eventModel, CancellationToken ct)
    {
        var serviceProvider = Resolve<IServiceProvider>();
        await using var scope = serviceProvider.CreateAsyncScope();
        var activeReservationService = scope.ServiceProvider.GetRequiredService<IActiveReservationFinder>();
        var elasticClient = scope.ServiceProvider.GetRequiredService<IElasticClient>();

        var document =
            await activeReservationService.GetReservationDocumentAsync(
                eventModel.Session.Id,
                eventModel.InitiatorUserId,
                eventModel.ReservationId,
                ct
            );

        if (document.Source.ReservationStatus is ReservationStatus.ActiveUnpaid)
        {
            throw new DomainException("reservation is already active");
        }

        var updatedDocument = document.Source with
        {
            ReservationStatus = ReservationStatus.ActiveUnpaid
        };
        var result = await elasticClient.UpdateAsync(
            new DocumentPath<ReservationDocument>(document.Id), u => u
                .Index(ElasticConfiguration.IndexConfig.ReservationEventIndex)
                .Doc(updatedDocument),
            ct
        );

        return result.Result == Result.Updated
            ? Task.CompletedTask
            : Task.FromException(new DomainException("reservation_document_creation_failed"));
    }
}