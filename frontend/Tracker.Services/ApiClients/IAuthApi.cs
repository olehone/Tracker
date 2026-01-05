using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;

namespace Tracker.Services.ApiClients;

public interface IAuthApi
{
    [Post("/api/auth/register")]
    Task<ApiResponse<TokensDto>> RegisterAsync(RegisterUserRequest request);

    [Post("/api/auth/login")]
    Task<ApiResponse<TokensDto>> LoginAsync(LoginUserRequest request);

    [Post("/api/auth/refresh-token")]
    Task<ApiResponse<TokensDto>> RefreshTokenAsync(RefreshTokenRequest request);
}