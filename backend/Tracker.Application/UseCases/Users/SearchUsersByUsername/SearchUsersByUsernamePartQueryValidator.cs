using FluentValidation;

namespace Tracker.Application.UseCases.Users.SearchUsersByUsername;

public class SearchUsersByUsernamePartQueryValidator : AbstractValidator<SearchUsersByUsernamePartQuery>
{

    public SearchUsersByUsernamePartQueryValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.AmountInPage)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.Page)
            .NotEmpty()
            .GreaterThanOrEqualTo(0);

    }

}
