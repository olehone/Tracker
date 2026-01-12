using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.GetAll;

public class GetWorkspacesQuery
    : PaginatedSearch, IRequest<Result<Paginated<WorkspaceSummaryDto>>>
{
}
