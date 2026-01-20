using Tracker.API.Requests;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public class WorkspaceUserService(IApiErrorHandler apiErrorHandler, IWorkspaceUserApi api) : IWorkspaceUserService
{
    public Task<Result<List<WorkspaceUserDto>>> GetUsersByWorkspaceAsync(Guid workspaceId)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetUsersByWorkspaceAsync(workspaceId));
    }

    public Task<Result<WorkspaceUserDto>> AddUserToWorkspaceAsync(Guid workspaceId, Guid userId, UserWorkspaceRole role)
    {
        return apiErrorHandler.ExecuteAsync(() => api.AddUserToWorkspaceAsync(workspaceId, userId, role));
    }

    public Task<Result> ChangeUserRoleAsync(Guid workspaceId, Guid userId, UserWorkspaceRole role)
    {
        return apiErrorHandler.ExecuteAsync(() => api.ChangeUserRoleAsync(workspaceId, userId, role));
    }

    public Task<Result> RemoveUserFromWorkspaceAsync(Guid workspaceId, Guid userId)
    {
        return apiErrorHandler.ExecuteAsync(() => api.RemoveUserFromWorkspaceAsync(workspaceId, userId));
    }
}