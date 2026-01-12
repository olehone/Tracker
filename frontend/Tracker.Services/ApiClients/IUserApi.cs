using System.Data;
using Microsoft.AspNetCore.Mvc;
using Refit;
using Tracker.API.Requests;
using Tracker.Domain.Dtos;

namespace Tracker.Services.ApiClients;

public interface IUserApi
{
    [Get("/api/users/me")]
    Task<ApiResponse<UserDto>> GetCurrentUserAsync();

    [Get("/api/users/{id}")]
    Task<ApiResponse<UserDto>> GetUserByIdAsync(Guid id);

    [Get("/api/users/all")]
    Task<ApiResponse<Paginated<UserDto>>> GetUsersAsync([Query] PaginatedSearchRequest request);

    [Get("/api/users/{id}/workspaces/all")]
    Task<ApiResponse<Paginated<WorkspaceSummaryDto>>> GetAllUserWorkspacesAsync([FromRoute] Guid id,
        [FromQuery] PaginatedSearchRequest request);

    [Get("/api/users/{id}/workspaces")]
    Task<ApiResponse<Paginated<WorkspaceSummaryDto>>> GetMutualWorkspacesAsync([FromRoute] Guid id,
        [FromQuery] PaginatedSearchRequest request);
}