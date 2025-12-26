using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public class UserService(
    IUserApi api)
    : IUserService
{
    public async Task<UserDto?> GetCurrentUserAsync()
    {
        return await api.GetCurrentUserAsync();
    }
}