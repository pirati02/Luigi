using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using Reservation.Api.Feature.MakeDraftReservation.ReservationCreated;
using Reservation.Api.Service;
using Reservation.Domain.ValueObjects;
using Reservation.EventStore;
using Reservation.Model.ValueObjects;
using Reservation.Postgres;
using SharedKernel;

namespace Reservation.Api.Feature.MakeDraftReservation;

[HttpPost("reservation")]
[AllowAnonymous]
public class MakeReservationCommandEndpoint : Endpoint<MakeReservationCommand>
{
    private readonly AggregateRepository _aggregateRepository;
    private readonly ReservationConfigurationDbContext _dbContext;
    private readonly IActiveReservationFinder _activeReservationFinder;
    private readonly IProducingService _producingService;
    private readonly string _registerExchange = "user.api.exchange";
    private readonly string _registerRoutingKey = "register.routing-key";

    public MakeReservationCommandEndpoint(
        AggregateRepository aggregateRepository,
        ReservationConfigurationDbContext dbContext,
        IActiveReservationFinder activeReservationFinder,
        IProducingService producingService
    )
    {
        _aggregateRepository = aggregateRepository;
        _dbContext = dbContext;
        _activeReservationFinder = activeReservationFinder;
        _producingService = producingService;
    }

    public override async Task HandleAsync(MakeReservationCommand req, CancellationToken ct)
    {
        var id = Guid.NewGuid();
        await _producingService.SendAsync(new
        {
            Id = id,
            req.InitiatorUser.Email,
            req.InitiatorUser.FirstName,
            req.InitiatorUser.LastName,
            req.InitiatorUser.Mobile
        }, _registerExchange, _registerRoutingKey);

        if (
            await _activeReservationFinder.CheckDraftSessionExists(
                req.SeatModel.Row,
                req.SeatModel.Column,
                req.SessionId,
                ct
            )
        )
        {
            throw new DomainException("already reserved");
        }

        var sessionConfig =
            await _dbContext.Sessions.FirstOrDefaultAsync(a => a.Id == req.SessionId, ct);
        if (sessionConfig is null)
        {
            throw new DomainException("session does not exist");
        }

        var reservationSettings =
            ReservationSettings.Create(sessionConfig.Spot.Rows, sessionConfig.Date, sessionConfig.Time);

        var reservationTime = ReservationTime.From(
            DateOnly.FromDateTime(req.ReservationTime),
            TimeOnly.FromDateTime(req.ReservationTime),
            reservationSettings.Date,
            reservationSettings.Time
        );
        var seat = Seat.At(
            req.SeatModel.Row,
            req.SeatModel.Column,
            reservationSettings.RowsAndColumns
        );
        var user = InitiatorUser.From(
            id,
            Email.From(req.InitiatorUser.Email),
            new UserName(req.InitiatorUser.FirstName, req.InitiatorUser.LastName)
        );
        var session = Session.From(sessionConfig.Id);

        var reservation = Domain.Model.Reservation.CreateDraft(user, reservationTime, seat, session);
        await _aggregateRepository.SaveAsync(reservation, ct);

        await SendOkAsync(ct);
        await PublishAsync(
            new ReservationCreatedEvent(
                reservation.Id,
                reservation.Seat,
                reservation.ReservationTime,
                reservation.InitiatorUser,
                reservation.ReservationStatus,
                reservation.Session
            ),
            Mode.WaitForAll,
            ct
        );
    }
}