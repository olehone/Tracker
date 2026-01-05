using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction.Entities;

public interface IUserService
{
    Task<Result<UserDto>> GetCurrentUserAsync();
}