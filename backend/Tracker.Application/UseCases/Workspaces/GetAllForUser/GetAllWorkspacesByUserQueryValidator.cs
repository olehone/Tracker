using FluentValidation;

namespace Tracker.Application.UseCases.Workspaces.GetAllForUser;

public class GetAllWorkspacesByUserQueryValidator
    : PaginatedSearchValidator<GetAllWorkspacesByUserQuery>
{
    public GetAllWorkspacesByUserQueryValidator() : base()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
