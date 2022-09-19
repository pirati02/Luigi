using SharedKernel;

namespace Reservation.Model.Exception;

public class ReservationAlreadyExists: DomainException
{
    public ReservationAlreadyExists() : base("reservation already exists")
    {
    }
}