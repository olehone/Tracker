using FluentValidation;

namespace Tracker.Application.UseCases.Workspaces.GetById;

public class GetWorkspaceByIdQueryValidator : AbstractValidator<GetWorkspaceByIdQuery>
{

    public GetWorkspaceByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }

}
