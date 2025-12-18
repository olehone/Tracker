using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;

namespace Tracker.Services.ApiClients;

public interface IAuthApi
{
    [Post("/api/auth/register")]
    Task<TokensDto> RegisterAsync(RegisterUserRequest request);

    [Post("/api/auth/login")]
    Task<TokensDto> LoginAsync(LoginUserRequest request);
    
    [Post("/api/auth/refresh-token")]
    Task<TokensDto?> RefreshTokenAsync(RefreshTokenRequest request);
    
    [Get("/api/auth/me")]
    Task<UserDto?> GetCurrentUserAsync();
}
