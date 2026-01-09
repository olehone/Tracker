using FluentValidation;

namespace Tracker.Application.UseCases.Workspaces.GetAllMutual;

public class GetMutualWorkspacesQueryValidator
    : PaginatedSearchValidator<GetMutualWorkspacesQuery>
{
    public GetMutualWorkspacesQueryValidator() : base()
    {
        RuleFor(x => x.TargetUserId)
            .NotEmpty();
    }
}
