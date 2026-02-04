using FluentValidation;
using Tracker.Domain.Requests.Users;

namespace Tracker.WebApp.Components.Users;

public partial class UserSettingsDialog
{
    private sealed class UpdateUserModelValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserModelValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required")
                .MinimumLength(3)
                .WithMessage("Username must be at least 3 characters")
                .MaximumLength(50)
                .WithMessage("Username can't exceed 50 characters");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required")
                .MaximumLength(50)
                .WithMessage("First name can't exceed 50 characters");

            RuleFor(x => x.LastName)
                .MaximumLength(50)
                .WithMessage("Last name can't exceed 50 characters");
        }
    }
}
