using FluentValidation;
using Microsoft.Extensions.Options;
using Tracker.Domain.Options;

namespace Tracker.Application.UseCases.Workspaces.Update;

public class UpdateWorkspaceCommandValidator
    : AbstractValidator<UpdateWorkspaceCommand>
{
    public UpdateWorkspaceCommandValidator(IOptions<EntityOptions> options)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(options.Value.TitleMaximumLength);

        RuleFor(x => x.Description)
            .MaximumLength(options.Value.DescriptionMaximumLength);
    }
}