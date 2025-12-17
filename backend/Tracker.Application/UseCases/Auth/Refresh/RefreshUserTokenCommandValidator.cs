using Tracker.Domain.Options;
using FluentValidation;

namespace Tracker.Application.UseCases.Auth.Refresh;

public class RefreshUserTokenCommandValidator : AbstractValidator<RefreshUserTokenCommand>
{
    public RefreshUserTokenCommandValidator()
    {
        RuleFor(t => t.RefreshToken)
            .NotEmpty();
    }
}
