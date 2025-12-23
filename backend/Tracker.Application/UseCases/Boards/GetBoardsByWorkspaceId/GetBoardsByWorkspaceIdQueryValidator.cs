using FluentValidation;

namespace Tracker.Application.UseCases.Boards.GetBoardsByWorkspaceId;

public class GetBoardsByWorkspaceIdQueryValidator 
    : AbstractValidator<GetBoardsByWorkspaceIdQuery>
{
    public GetBoardsByWorkspaceIdQueryValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();
    }
}
