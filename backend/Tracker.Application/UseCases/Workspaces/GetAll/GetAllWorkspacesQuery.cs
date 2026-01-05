using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.GetAll;

public class GetAllWorkspacesQuery : IRequest<Result<List<WorkspaceDto>>>
{
}
