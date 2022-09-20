using Reservation.Model.ValueObjects;

namespace Reservation.Domain.ValueObjects;

public class InitiatorUser: IEquatable<InitiatorUser>
{
    public Guid Id { get; private set; }
    public Email Email { get; private init; }
    public Mobile Mobile { get; private init; }
    public UserName UserName { get; private init; } 


    private InitiatorUser(Guid id, Email email, UserName userName)
    {
        Id = id;
        Email = email;
        UserName = userName;
        // Mobile = mobile;
    }

    public static InitiatorUser From(Guid id, Email email, UserName userName)
    {
        return new InitiatorUser(id, email, userName);
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