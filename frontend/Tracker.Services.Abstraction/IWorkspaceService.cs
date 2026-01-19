using Tracker.API.Requests;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Workspace;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IWorkspaceService
{
    Task<Result<WorkspaceFullDto>> GetByIdAsync(Guid id);
    Task<Result> UpdateAsync(Guid id, UpdateWorkspaceRequest request);
    Task<Result<Paginated<WorkspaceSummaryDto>>> GetAsync(PaginatedSearchRequest request);
    Task<Result<List<WorkspaceSummaryDto>>> GetForCurrentUserAsync();
    Task<Result<WorkspaceSummaryDto>> CreateAsync(CreateWorkspaceRequest request);
}