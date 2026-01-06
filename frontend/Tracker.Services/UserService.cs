using Tracker.Domain.Dtos;
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
}