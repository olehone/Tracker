using Refit;
using Tracker.API.Requests;
using Tracker.Domain.Dtos;

namespace Tracker.Services.ApiClients;

public interface IWorkspaceUserApi
{
    [Get("/api/workspaces/{workspaceId}/users")]
    Task<IApiResponse<List<WorkspaceUserDto>>> GetUsersByWorkspaceAsync(Guid workspaceId);

    [Post("/api/workspaces/{workspaceId}/users/{userId}")]
    Task<IApiResponse<WorkspaceUserDto>> AddUserToWorkspaceAsync(Guid workspaceId, Guid userId,
        WorkspaceUserRoleRequest request);

    [Put("/api/workspaces/{workspaceId}/users/{userId}")]
    Task<IApiResponse> ChangeUserRoleAsync(Guid workspaceId, Guid userId,
        WorkspaceUserRoleRequest request);

    [Delete("/api/workspaces/{workspaceId}/users/{userId}")]
    Task<IApiResponse> RemoveUserFromWorkspaceAsync(Guid workspaceId, Guid userId);
}