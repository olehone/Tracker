using System.Security.Claims;
using Tracker.Domain.Requests;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Auth;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public sealed class AuthService(
    IAuthApi api,
    IAuthStorage storage,
    IJwtTokenReader jwtTokenReader) 
    : IAuthService
{

    public event Action? OnLogin;
    public event Action? OnLogout;

    public async Task LoginAsync(LoginUserRequest request)
    {
        var response = await api.LoginAsync(request);
        await storage.SetAsync(response);
        OnLogin?.Invoke();
    }
    public async Task RegisterAsync(RegisterUserRequest request)
    {
        var response = await api.RegisterAsync(request);
        await storage.SetAsync(response);
        OnLogin?.Invoke();
    }

    public async Task LogoutAsync()
    {
        await storage.ClearAsync();
        OnLogout?.Invoke();
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        var tokensDto = await storage.GetAsync();
        if (tokensDto is null)
        {
            return null;
        }

        if (jwtTokenReader.GetExpirationUtc(tokensDto.AccessToken) > DateTimeOffset.UtcNow.AddSeconds(30))
        {
            return tokensDto.AccessToken;
        }

        var refreshed = await api.RefreshTokenAsync(new RefreshTokenRequest()
        {
            RefreshToken = tokensDto.RefreshToken
        });

        if (refreshed == null)
        {
            await LogoutAsync();
            return null;
        }

        await storage.SetAsync(refreshed);

        return refreshed.AccessToken;
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
}
