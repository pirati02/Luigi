using SharedKernel;

namespace Reservation.Model.Exception;

public class ReservationTimeMismatchException : DomainException
{
    public ReservationTimeMismatchException():base("Reservation date and time is invalid")
    {
        
    }
}