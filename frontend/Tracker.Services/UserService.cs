using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;
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
    public Task<Result<UserDto>> GetCurrentAsync()
    {
        return apiErrorHandler.ExecuteAsync(api.GetCurrentAsync);
    }

    public Task<Result<Paginated<UserDto>>> GetAsync(PaginatedSearchRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetAsync(request));
    }

    public Task<Result<UserDto>> GetByIdAsync(Guid id)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetByIdAsync(id));
    }

    public Task<Result<Paginated<WorkspaceSummaryDto>>> GetAllWorkspacesAsync(
        Guid id, PaginatedSearchRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetAllWorkspacesAsync(id, request));
    }

    public Task<Result<Paginated<WorkspaceSummaryDto>>> GetMutualWorkspacesAsync(
        Guid id, PaginatedSearchRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetMutualWorkspacesAsync(id, request));
    }
}