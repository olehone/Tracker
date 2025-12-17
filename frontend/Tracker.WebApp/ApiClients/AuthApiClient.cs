using Refit;
using Microsoft.AspNetCore.Mvc;
using Tracker.Domain.Entities.Commands;
using Tracker.Domain.Entities.Queries;

namespace Tracker.WebApp.ApiClients;

public interface AuthApiClient
{
    [Get("/api/auth/register")]
    Task<IActionResult> RegisterAsync(RegisterUserCommand request);
    [Post("api/auth/login")]
    Task<IActionResult> LoginAsync(LoginUserCommand request);
    [Post("api/auth/refresh-token")]
    Task<IActionResult> RefreshTokenAsync(RefreshUserTokenCommand request);
    [Get("api/auth/me")]
    Task<IActionResult> GetCurrentUser(GetCurrentUserQuery request);
}
