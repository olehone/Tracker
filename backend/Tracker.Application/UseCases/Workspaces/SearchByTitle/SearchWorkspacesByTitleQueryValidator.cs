using FluentValidation;

namespace Tracker.Application.UseCases.Workspaces.SearchByTitle;

public class SearchWorkspacesByTitleQueryValidator : AbstractValidator<SearchWorkspacesByTitleQuery>
{
    public SearchWorkspacesByTitleQueryValidator()
    {
        RuleFor(x => x.Title)
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