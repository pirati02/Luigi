namespace Reservation.Domain.ValueObjects;

public class InitiatorUser: IEquatable<InitiatorUser>
{
    public Guid Id { get; private set; }
    // public Email Email { get; private set; }
    // public Mobile Mobile { get; private set; }


    private InitiatorUser(Guid id)
    {
        Id = id;
        // Email = email;
        // Mobile = mobile;
    }

    public static InitiatorUser From(Guid id)
    {
        return new InitiatorUser(id);
    }

    public bool Equals(InitiatorUser? other)
    {
        return other?.Id == Id;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as InitiatorUser);
    }
}