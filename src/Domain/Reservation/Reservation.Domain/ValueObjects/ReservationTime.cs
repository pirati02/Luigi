using Reservation.Domain.Model;
using Reservation.Model.Exception;

namespace Reservation.Domain.ValueObjects;

public class ReservationTime: IEquatable<ReservationTime>
{
    public DateOnly Date { get; }
    public TimeOnly Time { get; }

    private ReservationTime(DateOnly date, TimeOnly time)
    {
        Date = date;
        Time = time;
    }

    public static ReservationTime From(DateOnly date, TimeOnly time, DateOnly dateToValidate, TimeOnly timeToValidate)
    {
        var reservationTime = new ReservationTime(date, time);
        
        if (!reservationTime.IsValid(dateToValidate, timeToValidate))
        {
            throw new ReservationTimeMismatchException();
        }
        return reservationTime;
    }

    private bool IsValid(DateOnly settingsDate, TimeOnly settingsTime)
    {
        return Date == settingsDate && Time == settingsTime;
    }

    public bool Equals(ReservationTime? other)
    {
        if (other is not null)
        {
            return other.Date == Date && other.Time == Time;
        }

        return ReferenceEquals(other, this);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as ReservationTime);
    }

    public static ReservationTime From(DateTime reservationDateTime)
    {
        return new ReservationTime(DateOnly.FromDateTime(reservationDateTime), TimeOnly.FromDateTime(reservationDateTime));
    }

    public DateTime AsDateTime()
    {
        return Date.ToDateTime(Time);
    }
}