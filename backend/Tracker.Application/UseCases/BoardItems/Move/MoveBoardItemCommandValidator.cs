using FluentValidation;

namespace Tracker.Application.UseCases.BoardItems.Move;

public class MoveBoardItemCommandValidator : AbstractValidator<MoveBoardItemCommand>
{
    public MoveBoardItemCommandValidator()
    {
        RuleFor(bi => bi.BoardItemId)
            .NotEmpty();

        RuleFor(bl => bl.Position)
            .GreaterThan(0);
    }
}
