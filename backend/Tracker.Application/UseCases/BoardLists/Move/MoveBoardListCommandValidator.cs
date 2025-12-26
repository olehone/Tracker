using FluentValidation;

namespace Tracker.Application.UseCases.BoardLists.Move;

public class MoveBoardListCommandValidator : AbstractValidator<MoveBoardListCommand>
{
    public MoveBoardListCommandValidator()
    {
        RuleFor(bl => bl.BoardListId)
            .NotEmpty();

        RuleFor(bl => bl.Position)
            .GreaterThan(0);
    }
}
