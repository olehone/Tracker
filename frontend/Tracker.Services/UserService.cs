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
    public async Task<Result<UserDto>> GetCurrentUserAsync()
    {
        return await apiErrorHandler.ExecuteAsync(api.GetCurrentUserAsync);
    }

    public async Task<Result<Paginated<UserDto>>> GetUsersAsync(PaginatedSearchRequest request)
    {
        return await apiErrorHandler.ExecuteAsync(request, api.GetUsersAsync);
    }

    public async Task<Result<UserDto>> GetUserByIdAsync(GetByIdRequest request)
    {
        return await apiErrorHandler.ExecuteAsync(request.Id, api.GetUserByIdAsync);
    }
}