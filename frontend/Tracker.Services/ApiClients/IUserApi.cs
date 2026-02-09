using Microsoft.AspNetCore.Mvc;
using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;
using Tracker.Domain.Requests.Users;

namespace Tracker.Services.ApiClients;

public interface IUserApi
{
    [Get("/api/users/{id}")]
    Task<IApiResponse<UserDto>> GetByIdAsync(Guid id);

    [Put("/api/users/{id}")]
    Task<IApiResponse> UpdateAsync(Guid id, UpdateUserRequest request);

    [Get("/api/users/me")]
    Task<IApiResponse<UserDto>> GetCurrentAsync();

    [Get("/api/users/all")]
    Task<IApiResponse<Paginated<UserDto>>> GetAsync([Query] PaginatedSearchRequest request);

    [Get("/api/users/{id}/workspaces/all")]
    Task<IApiResponse<Paginated<WorkspaceSummaryDto>>> GetAllWorkspacesAsync([FromRoute] Guid id,
        [FromQuery] PaginatedSearchRequest request);

    [Get("/api/users/{id}/workspaces")]
    Task<IApiResponse<Paginated<WorkspaceSummaryDto>>> GetMutualWorkspacesAsync([FromRoute] Guid id,
        [FromQuery] PaginatedSearchRequest request);

    [Get("/api/users/{id}/avatar")]
    Task<IApiResponse<string>> GetAvatarUrlAsync(Guid id);

    [Multipart]
    [Post("/api/users/{id}/avatar")]
    Task<IApiResponse<string>> UploadAvatarAsync(Guid Id, [AliasAs("File")] StreamPart file);

    [Delete("/api/users/{id}/avatar")]
    Task<IApiResponse> DeleteAvatarAsync(Guid id);
}