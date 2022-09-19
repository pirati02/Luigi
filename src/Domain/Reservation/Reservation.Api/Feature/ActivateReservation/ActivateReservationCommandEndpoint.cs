using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Reservation.Api.Feature.ActivateReservation.ReservationActivated;
using Reservation.Api.Service;
using Reservation.Domain.Model;
using Reservation.Infrastructure;
using SharedKernel;

namespace Reservation.Api.Feature.ActivateReservation;

[HttpPost("reservation/activate")]
[AllowAnonymous]
public class ActivateReservationCommandEndpoint : Endpoint<ActivateReservationCommand>
{
    private readonly AggregateRepository _aggregateRepository;
    private readonly IActiveReservationFinder _activeReservationFinder;

    public ActivateReservationCommandEndpoint(
        AggregateRepository aggregateRepository,
        IActiveReservationFinder activeReservationFinder
    )
    {
        _aggregateRepository = aggregateRepository;
        _activeReservationFinder = activeReservationFinder;
    }

    public override async Task HandleAsync(ActivateReservationCommand req, CancellationToken ct)
    {
        var activeSession =
            await _activeReservationFinder.GetReservationDocumentAsync(req.SessionId, req.InitiatorUserId,
                req.ReservationId, ct);
        if (activeSession is null)
        {
            throw new DomainException("reservation for session not found");
        }

        if (activeSession.Source.ReservationStatus is ReservationStatus.ActiveUnpaid or ReservationStatus.ActiveAsPerPaid)
        {
            throw new DomainException("reservation is already active");
        }

        var reservation = await _aggregateRepository.LoadAsync<Domain.Model.Reservation>(activeSession.Source.ReservationId);
        reservation.MakeActive();
        await _aggregateRepository.SaveAsync(reservation, ct);

        await PublishAsync(
            new ReservationActivatedEvent(
                reservation.Id,
                reservation.Session,
                reservation.InitiatorUser.Id
            ),
            Mode.WaitForAll,
            ct
        );
    }
}