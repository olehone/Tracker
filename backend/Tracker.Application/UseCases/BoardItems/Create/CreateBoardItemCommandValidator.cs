using FluentValidation;
using Microsoft.Extensions.Options;
using Tracker.Domain.Options;

namespace Tracker.Application.UseCases.BoardItems.Create;

public class CreateBoardItemCommandValidator : AbstractValidator<CreateBoardItemCommand>
{
    public CreateBoardItemCommandValidator(IOptions<EntityOptions> options)
    {
        RuleFor(bi => bi.BoardListId)
            .NotEmpty();

        RuleFor(bi => bi.Title)
            .NotEmpty()
            .MaximumLength(options.Value.TitleMaximumLength);

        RuleFor(x => x.Description)
            .MaximumLength(options.Value.DescriptionMaximumLength);
    }
}
