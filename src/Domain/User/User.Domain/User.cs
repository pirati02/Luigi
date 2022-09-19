namespace User.Domain;

public record User(
    Guid Id,
    UserName UserName, 
    Email Email,
    Mobile Mobile
);

public readonly record struct Mobile(string Value, int? CountryCode);
public readonly record struct Email(string Value);
public readonly record struct UserName(string FirstName, string LastName);