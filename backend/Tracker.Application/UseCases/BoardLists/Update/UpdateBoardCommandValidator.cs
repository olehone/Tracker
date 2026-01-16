using FluentValidation;
using Microsoft.Extensions.Options;
using Tracker.Domain.Options;

namespace Tracker.Application.UseCases.BoardLists.Update;

public class UpdateBoardListCommandValidator
    : AbstractValidator<UpdateBoardListCommand>
{
    public UpdateBoardListCommandValidator(IOptions<EntityOptions> options)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(options.Value.TitleMaximumLength);

        RuleFor(x => x.Description)
            .MaximumLength(options.Value.DescriptionMaximumLength);
    }
}