using FluentValidation;

namespace Tracker.Application.UseCases.Boards.GetByWorkspaceId;

public class GetBoardsByWorkspaceIdQueryValidator 
    : AbstractValidator<GetBoardsByWorkspaceIdQuery>
{
    public GetBoardsByWorkspaceIdQueryValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();
    }
}
