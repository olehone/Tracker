using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.GetAllForUser;

public class GetAllWorkspacesByUserQuery
    : PaginatedSearch, IRequest<Result<Paginated<WorkspaceSummaryDto>>>
{
    public required Guid Id { get; set; }
}
