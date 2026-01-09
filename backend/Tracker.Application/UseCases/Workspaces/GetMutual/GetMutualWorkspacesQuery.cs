using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.GetAllMutual;

public class GetMutualWorkspacesQuery
    : PaginatedSearch, IRequest<Result<Paginated<WorkspaceSummaryDto>>>
{
    public required Guid TargetUserId { get; set; }
}
