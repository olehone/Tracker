using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Workspace;

namespace Tracker.Services.ApiClients;

public interface IWorkspaceApi
{

    [Get("/api/workspaces/{id}")]
    Task<ApiResponse<WorkspaceFullDto>> GetWorkspaceByIdAsync(Guid id);
    [Get("/api/workspaces/")]
    Task<ApiResponse<List<WorkspaceSummaryDto>>> GetWorkspacesForCurrentUserAsync();

    [Post("/api/workspaces/")]
    Task<ApiResponse<WorkspaceSummaryDto>> CreateWorkspaceAsync(CreateWorkspaceRequest request);
}