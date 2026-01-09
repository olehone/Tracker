using Refit;
using Tracker.API.Requests;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Workspace;

namespace Tracker.Services.ApiClients;

public interface IWorkspaceApi
{

    [Get("/api/workspaces/{id}")]
    Task<ApiResponse<WorkspaceFullDto>> GetWorkspaceByIdAsync(Guid id);

    [Put("/api/workspaces/{id}/settings")]
    Task<ApiResponse<object>> UpdateAsync(Guid id, [Body] UpdateWorkspaceRequest request);

    [Get("/api/workspaces/all")]
    Task<ApiResponse<Paginated<WorkspaceSummaryDto>>> GetWorkspacesAsync([Query] PaginatedSearchRequest request);

    [Get("/api/workspaces/my")]
    Task<ApiResponse<List<WorkspaceSummaryDto>>> GetWorkspacesForCurrentUserAsync();

    [Post("/api/workspaces/")]
    Task<ApiResponse<WorkspaceSummaryDto>> CreateWorkspaceAsync(CreateWorkspaceRequest request);
}