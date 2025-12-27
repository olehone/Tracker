using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Common;
using Tracker.Domain.Requests.Workspace;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public class WorkspaceService(IWorkspaceApi api) : IWorkspaceService
{
    public Task<WorkspaceDto> CreateWorkspaceAsync(CreateWorkspaceRequest request)
        => api.CreateWorkspaceAsync(request);
    public Task<WorkspaceDto> GetWorkspaceByIdAsync(GetByIdRequest request)
        => api.GetWorkspaceByIdAsync(request.Id);
    public Task<List<WorkspaceDto>> GetWorkspacesAsync()
        => api.GetWorkspacesAsync();
}
