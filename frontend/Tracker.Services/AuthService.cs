using System.Security.Claims;
using Tracker.Domain.Requests;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction.Auth;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public sealed class AuthService(
    IApiErrorHandler apiErrorHandler,
    IAuthApi api,
    IAuthStorage storage,
    IJwtTokenReader jwtTokenReader)
    : IAuthService
{
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    public event Action? AuthStateChanged;

    public async Task<Result> LoginAsync(LoginUserRequest request)
    {
        var result = await apiErrorHandler.ExecuteAsync(() => api.LoginAsync(request));
        if (result.IsFailure)
        {
            return result.Error;
        }

        await storage.SetAsync(result.Value);

        AuthStateChanged?.Invoke();

        return Result.Success();
    }

    public async Task<Result> RegisterAsync(RegisterUserRequest request)
    {
        var result = await apiErrorHandler.ExecuteAsync(() => api.RegisterAsync(request));
        if (result.IsFailure)
        {
            return result.Error;
        }

        await storage.SetAsync(result.Value);
        AuthStateChanged?.Invoke();

        return Result.Success();
    }
    
    public async Task LogoutAsync()
    {
        await storage.ClearAsync();
        AuthStateChanged?.Invoke();
    }

    public async Task<ClaimsPrincipal> GetPrincipalAsync()
    {
        var accessToken = await GetAccessTokenAsync();
        if (accessToken is null)
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        var claims = jwtTokenReader.ReadClaims(accessToken);
        var identity = new ClaimsIdentity(claims, "jwt");
        return new ClaimsPrincipal(identity);
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        var tokensDto = await storage.GetAsync();
        if (tokensDto is null)
        {
            return null;
        }

        if (jwtTokenReader.GetExpirationUtc(tokensDto.AccessToken)
            > DateTimeOffset.UtcNow.AddSeconds(30))
        {
            return tokensDto.AccessToken;
        }

        try
        {
            await _refreshLock.WaitAsync();
            tokensDto = await storage.GetAsync();

            if (tokensDto is null)
            {
                return null;
            }

            if (jwtTokenReader.GetExpirationUtc(tokensDto.AccessToken)
                > DateTimeOffset.UtcNow.AddSeconds(30))
            {
                return tokensDto.AccessToken;
            }

            var request = new RefreshTokenRequest
            {
                RefreshToken = tokensDto.RefreshToken
            };

            var result = await apiErrorHandler.ExecuteAsync(() => api.RefreshTokenAsync(request));

            if (result == null || result.IsFailure)
            {
                return null;
            }

            await storage.SetAsync(result.Value);

            return result.Value.AccessToken;
        }
        catch (Exception)
        {
            return null;
        }
        finally
        {
            _refreshLock.Release();
        }
    }
}