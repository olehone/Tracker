using System.Data;
using Microsoft.AspNetCore.Mvc;
using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;

namespace Tracker.Services.ApiClients;

public interface IUserApi
{
    [Get("/api/users/me")]
    Task<IApiResponse<UserDto>> GetCurrentAsync();

    [Get("/api/users/{id}")]
    Task<IApiResponse<UserDto>> GetByIdAsync(Guid id);

    [Get("/api/users/all")]
    Task<IApiResponse<Paginated<UserDto>>> GetAsync([Query] PaginatedSearchRequest request);

    [Get("/api/users/{id}/workspaces/all")]
    Task<IApiResponse<Paginated<WorkspaceSummaryDto>>> GetAllWorkspacesAsync([FromRoute] Guid id,
        [FromQuery] PaginatedSearchRequest request);

    [Get("/api/users/{id}/workspaces")]
    Task<IApiResponse<Paginated<WorkspaceSummaryDto>>> GetMutualWorkspacesAsync([FromRoute] Guid id,
        [FromQuery] PaginatedSearchRequest request);
}