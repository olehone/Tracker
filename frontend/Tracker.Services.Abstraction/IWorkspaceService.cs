using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Workspace;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction.Entities;

public interface IWorkspaceService
{
    Task<Result<WorkspaceDto>> GetWorkspaceByIdAsync(Guid id);
    Task<Result<List<WorkspaceDto>>> GetWorkspacesAsync();
    Task<Result<WorkspaceDto>> CreateWorkspaceAsync(CreateWorkspaceRequest request);
}