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
        return apiErrorHandler.ExecuteAsync(request, api.GetUsersAsync);
    }

    public Task<Result<UserDto>> GetUserByIdAsync(GetByIdRequest request)
    {
        return apiErrorHandler.ExecuteAsync(request.Id, api.GetUserByIdAsync);
    }

    public Task<Result<Paginated<WorkspaceSummaryDto>>> GetAllUserWorkspacesAsync(
        GetByIdRequest id, PaginatedSearchRequest request)
    {
        return apiErrorHandler.ExecuteAsync(id.Id, request, api.GetAllUserWorkspacesAsync);
    }

    public Task<Result<Paginated<WorkspaceSummaryDto>>> GetMutualWorkspacesAsync(
        GetByIdRequest id, PaginatedSearchRequest request)
    {
        return apiErrorHandler.ExecuteAsync(id.Id, request, api.GetMutualWorkspacesAsync);
    }
}