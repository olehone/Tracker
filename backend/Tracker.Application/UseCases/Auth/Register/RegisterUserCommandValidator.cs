using FluentValidation;
using Microsoft.Extensions.Options;
using Tracker.Domain.Options;

namespace Tracker.Application.UseCases.Auth.Register;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator(IOptions<RegistrationOptions> options)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(options.Value.PasswordMinimumLength);

        RuleFor(x => x.FirstName)
            .NotEmpty();

        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(options.Value.UsernameMinimumLength)
            .MaximumLength(options.Value.UsernameMaximumLength);
    }
}
