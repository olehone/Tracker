using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.GetForCurrentUser;

public class GetWorkspacesForCurrentUserQuery : IRequest<Result<List<WorkspaceSummaryDto>>>
{
}
