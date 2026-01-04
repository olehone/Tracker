using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.Create;

public class CreateWorkspaceCommand : IRequest<Result<WorkspaceDto>>
{
    public string Title { get; set;} = string.Empty;
    public string Description { get; set;} = string.Empty;
}
