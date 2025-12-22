using FluentValidation;

namespace Tracker.Application.UseCases.Workspaces.GetWorkspaceById;

public class GetWorkspaceByIdQueryValidator : AbstractValidator<GetWorkspaceByIdQuery>
{

    public GetWorkspaceByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }

}
