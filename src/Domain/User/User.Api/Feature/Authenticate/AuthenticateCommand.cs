namespace User.Api.Feature.Authenticate;

public class AuthenticateCommand
{
    public string Email { get; init; }
    public string Password { get; init; }
}