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

    [Get("/api/users")]
    Task<ApiResponse<Paginated<UserDto>>> GetUsersAsync([Query] PaginatedSearchRequest request);
}