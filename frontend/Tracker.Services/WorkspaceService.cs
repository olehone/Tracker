using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Common;
using Tracker.Domain.Requests.Workspace;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public class WorkspaceService(IWorkspaceApi api) : IWorkspaceService
{
    public Task<WorkspaceDto> CreateWorkspaceAsync(CreateWorkspaceRequest request)
        => api.CreateWorkspaceAsync(request);
    public Task<WorkspaceDto> GetWorkspaceByIdAsync(Guid id)
        => api.GetWorkspaceByIdAsync(id);
    public Task<List<WorkspaceDto>> GetWorkspacesAsync()
        => api.GetWorkspacesAsync();
}
