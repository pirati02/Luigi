using FastEndpoints;
using FluentValidation;
using Reservation.Api.Feature.MakeDraftReservation;

namespace Reservation.Api.Feature.ActivateReservation;

public class ActivateReservationCommandValidation : Validator<ActivateReservationCommand>
{
    public ActivateReservationCommandValidation()
    {
        RuleFor(a => a.SessionId)
            .NotEmpty()
            .NotNull();
        
        RuleFor(a => a.InitiatorUserId)
            .NotEmpty()
            .NotNull();
    }
}