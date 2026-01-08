using FluentValidation;
using Microsoft.Extensions.Options;
using Tracker.Domain.Options;

namespace Tracker.Application.UseCases.Boards.Update;

public class UpdateBoardCommandValidator
    : AbstractValidator<UpdateBoardCommand>
{
    public UpdateBoardCommandValidator(IOptions<EntityOptions> options)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(options.Value.TitleMaximumLength);

        RuleFor(x => x.Description)
            .MaximumLength(options.Value.DescriptionMaximumLength);
    }

}