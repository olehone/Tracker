using FluentValidation;
using Tracker.Application.UseCases.Boards.GetBoardById;

namespace Tracker.Application.UseCases.Boards.GetBoardById;

public class GetBoardByIdQueryValidator : AbstractValidator<GetBoardByIdQuery>
{
    public GetBoardByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
