using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.GetWorkspaces;

public class GetWorkspacesQuery : IRequest<Result<List<WorkspaceDto>>>
{
}
