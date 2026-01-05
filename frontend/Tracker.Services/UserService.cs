using Tracker.Domain.Dtos;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction.Entities;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services.Entities;

public class UserService(
    IApiErrorHandler apiErrorHandler,
    IUserApi api)
    : IUserService
{
    public async Task<Result<UserDto>> GetCurrentUserAsync()
        => await apiErrorHandler.ExecuteAsync(api.GetCurrentUserAsync);
    
}