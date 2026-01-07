using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Common;
using Tracker.Domain.Requests.Workspace;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IWorkspaceService
{
    Task<Result<WorkspaceFullDto>> GetWorkspaceByIdAsync(GetByIdRequest request);
    Task<Result> UpdateAsync(GetByIdRequest id, UpdateWorkspaceRequest request);
    Task<Result<List<WorkspaceSummaryDto>>> GetWorkspacesForCurrentUserAsync();
    Task<Result<WorkspaceSummaryDto>> CreateWorkspaceAsync(CreateWorkspaceRequest request);
}