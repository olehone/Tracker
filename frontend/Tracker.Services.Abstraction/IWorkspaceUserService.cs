using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IWorkspaceUserService
{
    Task<Result<List<WorkspaceUserDto>>> GetUsersByWorkspaceAsync(Guid workspaceId);
    Task<Result<WorkspaceUserDto>> AddUserToWorkspaceAsync(Guid workspaceId, Guid userId, WorkspaceUserRole role);
    Task<Result> ChangeUserRoleAsync(Guid workspaceId, Guid userId, WorkspaceUserRole role);
    Task<Result> RemoveUserFromWorkspaceAsync(Guid workspaceId, Guid userId);
}