using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using SharedKernel;
using User.Api.Service;

namespace User.Api.Feature.Register;

[HttpPost("users")]
[AllowAnonymous]
public class RegisterUserCommandEndpoint : Endpoint<RegisterUserCommand>
{
    private readonly IUserService _userService;

    public RegisterUserCommandEndpoint(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task HandleAsync(RegisterUserCommand req, CancellationToken ct)
    {
        var registeredUser = await _userService.FindByAsync(req.Email);
        if (registeredUser is not null)
        {
            throw new DomainException("user already exists");
        }

        await _userService.RegisterAsync(
            req.Id,
            req.FirstName,
            req.LastName,
            req.Email,
            req.Password,
            req.Mobile,
            req.CountryCode
        );

        if (HttpContext is not null)
        {
            await SendOkAsync(ct);
        }
    }
}