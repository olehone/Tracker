using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Common;
using Tracker.Domain.Requests.Workspace;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction.Entities;

public interface IWorkspaceService
{
    Task<Result<List<WorkspaceDto>>> GetWorkspacesAsync();
    Task<Result<WorkspaceDto>> GetWorkspaceByIdAsync(GetByIdRequest request);
    Task<Result<WorkspaceDto>> CreateWorkspaceAsync(CreateWorkspaceRequest request);
}