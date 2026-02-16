using FluentValidation;
using Microsoft.Extensions.Options;
using Tracker.Domain.Options;

namespace Tracker.Application.UseCases.ItemComments.Update;

public class UpdateItemCommentCommandValidator
    : AbstractValidator<UpdateItemCommentCommand>
{
    public UpdateItemCommentCommandValidator(IOptions<EntityOptions> options)
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(options.Value.DescriptionMaximumLength);
    }
}