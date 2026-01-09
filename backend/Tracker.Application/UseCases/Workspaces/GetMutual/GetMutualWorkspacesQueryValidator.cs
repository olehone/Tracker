using FluentValidation;

namespace Tracker.Application.UseCases.Workspaces.GetMutual;

public class GetMutualWorkspacesQueryValidator
    : PaginatedSearchValidator<GetMutualWorkspacesQuery>
{
    public GetMutualWorkspacesQueryValidator() : base()
    {
        RuleFor(x => x.TargetUserId)
            .NotEmpty();
    }
}
