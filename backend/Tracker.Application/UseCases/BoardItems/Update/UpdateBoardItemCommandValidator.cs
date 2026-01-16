using FluentValidation;
using Microsoft.Extensions.Options;
using Tracker.Domain.Options;

namespace Tracker.Application.UseCases.BoardItems.Update;

public class UpdateBoardItemCommandValidator
    : AbstractValidator<UpdateBoardItemCommand>
{
    public UpdateBoardItemCommandValidator(IOptions<EntityOptions> options)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(options.Value.TitleMaximumLength);

        RuleFor(x => x.Description)
            .MaximumLength(options.Value.DescriptionMaximumLength);
    }
}