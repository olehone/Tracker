using Tracker.Domain.Entities;
using Tracker.Domain.Entities.Commands;
using Tracker.Services.Abstraction;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public sealed class AuthService : IAuthService
{
    private readonly IAuthApi _api;
    private readonly IAuthStorage _storage;
    private readonly IJwtTokenReader _jwtTokenReader;

    public AuthService(IAuthApi api, IAuthStorage storage, IJwtTokenReader jwtTokenReader)
    {
        _api = api;
        _storage = storage;
        _jwtTokenReader = jwtTokenReader;
    }

    public async Task LoginAsync(LoginUserCommand command)
    {
        var response = await _api.LoginAsync(command);
        await _storage.SetAsync(response);
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        var authSession = await _storage.GetAsync();
        if (authSession is null)
        {
            return null;
        }

        if (_jwtTokenReader.GetExpirationUtc(authSession.AccessToken) > DateTimeOffset.UtcNow.AddSeconds(30))
        {
            return authSession.AccessToken;
        }

        if (authSession.RefreshToken.ExpiresAt < DateTimeOffset.UtcNow)
        {
            return null;
        }

        var refreshed = await _api.RefreshTokenAsync(new RefreshUserTokenCommand()
        {
            RefreshToken = authSession.RefreshToken.Token
        });

        await _storage.SetAsync(refreshed);

        return refreshed.AccessToken;
    }

    public Task LogoutAsync()
        => _storage.ClearAsync();
}
