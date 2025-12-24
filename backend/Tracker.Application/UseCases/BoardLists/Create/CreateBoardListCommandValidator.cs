using FluentValidation;
using Microsoft.Extensions.Options;
using Tracker.Domain.Options;

namespace Tracker.Application.UseCases.BoardLists.Create;

public class CreateBoardListCommandValidator : AbstractValidator<CreateBoardListCommand>
{
    public CreateBoardListCommandValidator(IOptions<EntityOptions> options)
    {
        RuleFor(bl => bl.BoardId)
            .NotEmpty();

        RuleFor(bl => bl.Title)
            .NotEmpty()
            .MaximumLength(options.Value.TitleMaximumLength);

        RuleFor(x => x.Description)
            .MaximumLength(options.Value.DescriptionMaximumLength);
    }
}