using Microsoft.Extensions.Options;
using Tracker.Domain.Options;
using FluentValidation;

namespace Tracker.Application.UseCases.Auth.Login;

internal class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator(IOptions<LoginOptions> options)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(options.Value.PasswordMinimumLength)
            .WithMessage($"Password must be at least {options.Value.PasswordMinimumLength} characters long");
    }
}
