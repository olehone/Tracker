using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Workspace;

namespace Tracker.Services.ApiClients;

public interface IWorkspaceApi
{
    [Get("/api/workspaces/{id}")]
    Task<WorkspaceDto> GetWorkspaceByIdAsync(Guid id);

    [Get("/api/workspaces/")]
    Task<List<WorkspaceDto>> GetWorkspacesAsync();

    [Post("/api/workspaces/")]
    Task<WorkspaceDto> CreateWorkspaceAsync(CreateWorkspaceRequest request);
}
