using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.Create;

public class CreateWorkspaceCommand : IRequest<Result<WorkspaceFullDto>>
{
    public required string Title { get; set;}
}
