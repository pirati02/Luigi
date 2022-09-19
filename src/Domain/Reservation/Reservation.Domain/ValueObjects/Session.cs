namespace Reservation.Domain.ValueObjects;

public class Session : IEquatable<Session>
{
    private Session(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }

    public static Session From(Guid id)
    {
        return new Session(id);
    }

    public bool Equals(Session? other)
    {
        return other?.Id == Id;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Session);
    }
}