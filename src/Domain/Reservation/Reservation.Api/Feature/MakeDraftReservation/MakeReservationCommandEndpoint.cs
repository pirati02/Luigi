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
public class MakeReservationCommandEndpoint : Endpoint<MakeReservationCommand, Guid>
{
    private readonly AggregateRepository _aggregateRepository;
    private readonly ReservationConfigurationDbContext _dbContext;
    private readonly IActiveReservationFinder _activeReservationFinder;
    private readonly IProducingService _producingService;
    private readonly string _registerExchange = "user.api.exchange";

    public MakeReservationCommandEndpoint(
        AggregateRepository aggregateRepository,
        ReservationConfigurationDbContext dbContext,
        IActiveReservationFinder activeReservationFinder,
        IProducingService producingService,
        IConfiguration configuration
    )
    {
        _aggregateRepository = aggregateRepository;
        _dbContext = dbContext;
        _activeReservationFinder = activeReservationFinder;
        _producingService = producingService;
    }

    public override async Task HandleAsync(MakeReservationCommand req, CancellationToken ct)
    {
        await _producingService.SendAsync(new
        {
            req.InitiatorUser.Email,
            req.InitiatorUser.FirstName,
            req.InitiatorUser.LastName,
            req.InitiatorUser.Mobile
        }, _registerExchange, "register.routing-key");

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
            req.InitiatorUser.Id,
            Email.From(req.InitiatorUser.Email),
            new UserName(req.InitiatorUser.FirstName, req.InitiatorUser.LastName)
        );
        var session = Session.From(sessionConfig.Id);

        var reservation = Domain.Model.Reservation.CreateDraft(user, reservationTime, seat, session);
        await _aggregateRepository.SaveAsync(reservation, ct);

        await SendOkAsync(reservation.Id, ct);
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