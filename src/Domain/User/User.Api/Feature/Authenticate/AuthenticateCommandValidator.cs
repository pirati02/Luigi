using FastEndpoints;
using FluentValidation;

namespace User.Api.Feature.Authenticate;

public class AuthenticateCommandValidator: Validator<AuthenticateCommand>
{
    public AuthenticateCommandValidator()
    {
        RuleFor(a => a.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();

        RuleFor(a => a.Password)
            .NotNull()
            .NotEmpty();
    }
}