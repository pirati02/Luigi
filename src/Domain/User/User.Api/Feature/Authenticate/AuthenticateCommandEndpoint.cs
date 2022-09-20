using FastEndpoints;

namespace User.Api.Feature.Authenticate;

[HttpPost("user/login")]
public class AuthenticateCommandEndpoint: FastEventHandler<AuthenticateCommand>
{
    public override Task HandleAsync(AuthenticateCommand eventModel, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}