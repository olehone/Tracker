using FluentValidation;
using Microsoft.Extensions.Options;
using Tracker.Domain.Options;

namespace Tracker.Application.UseCases.Workspaces.Create;

public class CreateWorkspaceCommandValidator : AbstractValidator<CreateWorkspaceCommand>
{
    public CreateWorkspaceCommandValidator(IOptions<EntityOptions> options)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(options.Value.TitleMaximumLength);
    }
}
