using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;
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

    public async Task LoginAsync(LoginUserRequest request)
    {
        var response = await _api.LoginAsync(request);
        await _storage.SetAsync(response);
    }
    public async Task RegisterAsync(RegisterUserRequest request)
    {
        var response = await _api.RegisterAsync(request);
        await _storage.SetAsync(response);
    }

    public async Task LogoutAsync()
    { 
        await _storage.ClearAsync();
    }

    public async Task<UserDto?> GetCurrentUserAsync()
    {
        var user = await _api.GetCurrentUser();
        return user;
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        var tokensDto = await _storage.GetAsync();
        if (tokensDto is null)
        {
            return null;
        }

        if (_jwtTokenReader.GetExpirationUtc(tokensDto.AccessToken) > DateTimeOffset.UtcNow.AddSeconds(30))
        {
            return tokensDto.AccessToken;
        }

        var refreshed = await _api.RefreshTokenAsync(new RefreshTokenRequest()
        {
            RefreshToken = tokensDto.RefreshToken
        });
        
        if(refreshed == null)
        {
            await LogoutAsync();
            return null;
        }

        await _storage.SetAsync(refreshed);

        return refreshed.AccessToken;
    }

}
