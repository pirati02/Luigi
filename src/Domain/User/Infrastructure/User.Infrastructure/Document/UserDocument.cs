namespace User.Infrastructure.Document;

public record UserDocument(
    string FirstName,
    string LastName,
    string Email,
    string Mobile,
    int? CountryCode
);