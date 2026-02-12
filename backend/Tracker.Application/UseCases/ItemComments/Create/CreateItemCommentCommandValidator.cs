using FluentValidation;
using Microsoft.Extensions.Options;
using Tracker.Application.UseCases.ItemComments.Create;
using Tracker.Domain.Options;

namespace Tracker.Application.UseCases.BoardItems.Create;

public class CreateItemCommentCommandValidator : AbstractValidator<CreateItemCommentCommand>
{
    public CreateItemCommentCommandValidator(IOptions<EntityOptions> options)
    {
        RuleFor(bi => bi.Content)
            .NotEmpty()
            .MaximumLength(options.Value.DescriptionMaximumLength);
    }
}
