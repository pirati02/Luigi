using FastEndpoints;
using FluentValidation;

namespace Reservation.Api.Feature.MakeDraftReservation;

public class MakeReservationCommandValidation : Validator<MakeReservationCommand>
{
    public MakeReservationCommandValidation()
    {
        RuleFor(a => a.SessionId)
            .NotEmpty()
            .NotNull();
        
        RuleFor(a => a.InitiatorUserId)
            .NotEmpty()
            .NotNull();

        RuleFor(a => a.SeatModel)
            .NotNull()
            .ChildRules(a =>
                {
                    a.RuleFor(b => b.Row)
                        .GreaterThan(0);
                    a.RuleFor(b => b.Column)
                        .GreaterThan(0);
                }
            );

        RuleFor(a => a.ReservationTime)
            .NotEmpty()
            .NotNull()
            .GreaterThan(DateTime.Now);
    }
}