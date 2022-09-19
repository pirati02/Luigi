using SharedKernel;

namespace Reservation.Domain.ValueObjects;

public class Seat : IEquatable<Seat>
{
    private const int DefaultValue = -1;
    public int Row { get; } = DefaultValue;
    public int Column { get; } = DefaultValue;

    private Seat(int row, int column)
    {
        Row = row;
        Column = column;
    }
 
    public static Seat At(int row, int column, IReadOnlyList<List<int>> rowsAndColumns)
    {
        var seat = new Seat(row, column);
        if (!seat.IsValid(rowsAndColumns))
        {
            throw new DomainException("seat mismatch");
        }

        return seat;
    }

    private bool IsValid(IReadOnlyList<List<int>> rowsAndColumns)
    {
        return Row > DefaultValue
               && Column > DefaultValue
               && Row < rowsAndColumns.Count
               && Column < rowsAndColumns[0].Count
               && rowsAndColumns[Row][Column] != DefaultValue;
    }

    public bool Equals(Seat? other)
    {
        return other?.Column == Column && other.Row == Row;
    }

    public override bool Equals(object obj)
    {
        if (obj is Seat seat)
        {
            return Equals(seat);
        }

        return ReferenceEquals(this, obj);
    }
}