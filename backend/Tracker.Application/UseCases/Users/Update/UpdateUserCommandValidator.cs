using FluentValidation;
using Microsoft.Extensions.Options;
using Tracker.Domain.Options;

namespace Tracker.Application.UseCases.Users.Update;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator(IOptions<RegistrationOptions> options)
    {
        RuleFor(x => x.Username)
          .NotEmpty()
            .MinimumLength(options.Value.UsernameMinimumLength)
            .MaximumLength(options.Value.UsernameMaximumLength);

        RuleFor(x => x.FirstName)
            .NotEmpty();
    }
}