using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;

namespace Tracker.Services.ApiClients;

public interface IAuthApi
{
    [Post("/api/auth/register")]
    Task<IApiResponse<TokensDto>> RegisterAsync(RegisterUserRequest request);

    [Post("/api/auth/login")]
    Task<IApiResponse<TokensDto>> LoginAsync(LoginUserRequest request);

    [Post("/api/auth/refresh-token")]
    Task<IApiResponse<TokensDto>> RefreshTokenAsync(RefreshTokenRequest request);
}