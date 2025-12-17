using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Entities.Commands;
using Tracker.Domain.Entities.Queries;

namespace Tracker.Services.ApiClients;

public interface IAuthApi
{
    [Get("/api/auth/register")]
    Task<AuthSession> RegisterAsync(RegisterUserCommand request);
    [Post("/api/auth/login")]
    Task<AuthSession> LoginAsync(LoginUserCommand request);
    [Post("/api/auth/refresh-token")]
    Task<AuthSession> RefreshTokenAsync(RefreshUserTokenCommand request);
    [Get("/api/auth/me")]
    Task<UserDto> GetCurrentUser(GetCurrentUserQuery request);
}
