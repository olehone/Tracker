using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Common;
using Tracker.Domain.Requests.Workspace;

namespace Tracker.Services;

public interface IWorkspaceService
{
    Task<WorkspaceDto> GetWorkspaceByIdAsync(Guid id);
    Task<List<WorkspaceDto>> GetWorkspacesAsync();
    Task<WorkspaceDto> CreateWorkspaceAsync(CreateWorkspaceRequest request);
}