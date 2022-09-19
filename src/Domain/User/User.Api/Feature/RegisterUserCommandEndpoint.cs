using FastEndpoints;
using SharedKernel;
using User.Api.Service;
using User.Domain;

namespace User.Api.Feature;

public class RegisterUserCommandEndpoint : Endpoint<RegisterUserCommand>
{
    private readonly IUserService _userService;

    public RegisterUserCommandEndpoint(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task HandleAsync(RegisterUserCommand req, CancellationToken ct)
    {
        var registeredUser = await _userService.FindBy(new Email(req.Email), ct);
        if (registeredUser.Source is not null)
        {
            throw new DomainException("user already exists");
        }

        var id = await _userService.RegisterAsync(
            req.FirstName,
            req.LastName,
            req.Mobile,
            req.Email,
            req.CountryCode,
            ct
        );
        
        await SendOkAsync(id, ct);
    }
}