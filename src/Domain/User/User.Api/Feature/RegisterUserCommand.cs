namespace User.Api.Feature;

public record RegisterUserCommand
{
    public string Id { get; set; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string Mobile { get; init; }
    public int? CountryCode { get; init; }
}