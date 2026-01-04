using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Workspace;

namespace Tracker.Services.ApiClients;

public interface IWorkspaceApi
{
    [Get("/api/workspaces/{id}")]
    Task<ApiResponse<WorkspaceDto>> GetWorkspaceByIdAsync(Guid id);

    [Get("/api/workspaces/")]
    Task<ApiResponse<List<WorkspaceDto>>> GetWorkspacesAsync();

    [Post("/api/workspaces/")]
    Task<ApiResponse<WorkspaceDto>> CreateWorkspaceAsync(CreateWorkspaceRequest request);
}
