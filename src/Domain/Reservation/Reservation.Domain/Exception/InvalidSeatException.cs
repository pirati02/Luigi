using SharedKernel;

namespace Reservation.Model.Exception;

public class InvalidSeatException: DomainException
{
    public InvalidSeatException() : base("Invalid seat is chosen")
    {
    }
}