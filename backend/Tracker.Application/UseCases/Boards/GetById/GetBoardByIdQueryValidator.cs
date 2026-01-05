using FluentValidation;
using Tracker.Application.UseCases.Boards.GetById;

namespace Tracker.Application.UseCases.Boards.GetById;

public class GetBoardByIdQueryValidator : AbstractValidator<GetBoardByIdQuery>
{
    public GetBoardByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
