using System.Text.RegularExpressions;
using FastEndpoints;
using FluentValidation;
using FluentValidation.Validators;

namespace User.Api.Feature.Register;

public class RegisterUserCommandValidator : Validator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(a => a.FirstName)
            .NotEmpty()
            .NotNull()
            .MinimumLength(3);

        RuleFor(a => a.LastName)
            .NotEmpty()
            .NotNull()
            .MinimumLength(3);

        RuleFor(a => a.Email)
            .NotEmpty()
            .NotNull()
            .EmailAddress();

        RuleFor(a => a.Mobile)
            .NotEmpty()
            .NotNull()
            .SetValidator(new PhoneNumberValidator<RegisterUserCommand>());
    }
}
 
public class PhoneNumberValidator<T> : PropertyValidator<T, string>
{
    public override bool IsValid(ValidationContext<T> context, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            context.AddFailure("invalid phone number");
            return false;
        }

        value = value.Trim();
        if (value.Contains(' '))
        {
            value = value.Replace(" ", "");
        }

        var match = Regex.Match(value, @"^[0-9]");
        if (!match.Success)
        {
            context.AddFailure("invalid phone number");
            return false;
        }

        return true;
    }

    public override string Name => "PhoneNumberValidator<T>";
}