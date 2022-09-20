using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using User.Api.Service;

namespace User.Api.Feature.Authenticate;

[HttpPost("user/login")]
[AllowAnonymous]
public class AuthenticateCommandEndpoint : Endpoint<AuthenticateCommand>
{
    private readonly IIdentityService _identityService;

    public AuthenticateCommandEndpoint(IIdentityService identityService)
    {
        // Validator<AuthenticateCommandValidator>();
        _identityService = identityService;
    }

    public override async Task HandleAsync(AuthenticateCommand req, CancellationToken ct)
    {
        await _identityService.AuthenticateAsync(req.Email, req.Password, HttpContext);
    }
}