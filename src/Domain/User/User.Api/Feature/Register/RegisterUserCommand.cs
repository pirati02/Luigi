namespace User.Api.Feature.Register;

public record RegisterUserCommand
{
    public string Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }
    public string Mobile { get; init; }
    public int? CountryCode { get; init; }
}