using Refit;
using Tracker.Domain.Dtos;

namespace Tracker.Services.ApiClients;

public interface IUserApi
{
    [Get("/api/users/me")]
    Task<UserDto?> GetCurrentUserAsync();
}
