using Tracker.Domain.Entities;
using Tracker.Domain.Entities.Commands;

namespace Tracker.Services.Abstraction;

public interface IAuthService
{
    Task LoginAsync(LoginUserCommand command);
    Task<string?> GetAccessTokenAsync();
    Task LogoutAsync();
}

