namespace Reservation.Settings.Infrastructure;

public class ReservationSettings
{
    public List<List<int>> RowsAndColumns { get; }
    public DateOnly Date { get; }
    public TimeOnly Time { get; }

    private ReservationSettings(List<List<int>> rowsAndColumns, DateOnly date, TimeOnly time)
    {
        RowsAndColumns = rowsAndColumns;
        Date = date;
        Time = time;
    }
    public static ReservationSettings Create(List<List<int>> rowsAndColumns, DateOnly date, TimeOnly time)
    {
        return new ReservationSettings(rowsAndColumns, date, time);
    }
}