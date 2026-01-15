using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Common;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public class UserService(
    IApiErrorHandler apiErrorHandler,
    IUserApi api)
    : IUserService
{
    public Task<Result<UserDto>> GetCurrentUserAsync()
    {
        return apiErrorHandler.ExecuteAsync(api.GetCurrentUserAsync);
    }

    public Task<Result<Paginated<UserDto>>> GetUsersAsync(PaginatedSearchRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetUsersAsync(request));
    }

    public Task<Result<UserDto>> GetUserByIdAsync(Guid id)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetUserByIdAsync(id));
    }

    public Task<Result<Paginated<WorkspaceSummaryDto>>> GetAllUserWorkspacesAsync(
        Guid id, PaginatedSearchRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetAllUserWorkspacesAsync(id, request));
    }

    public Task<Result<Paginated<WorkspaceSummaryDto>>> GetMutualWorkspacesAsync(
        Guid id, PaginatedSearchRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetMutualWorkspacesAsync(id, request));
    }
}