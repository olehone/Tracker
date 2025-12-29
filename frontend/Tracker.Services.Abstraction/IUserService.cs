using Tracker.Domain.Dtos;

namespace Tracker.Services.Abstraction;

public interface IUserService
{
    Task<UserDto?> GetCurrentUserAsync();
}