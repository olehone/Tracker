using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction.Entities;
using Tracker.Services.ApiClients;

namespace Tracker.Services.Entities;

public class UserService(
    IUserApi api)
    : IUserService
{
    public async Task<UserDto?> GetCurrentUserAsync()
    {
        return await api.GetCurrentUserAsync();
    }
}