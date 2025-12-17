using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Tracker.Services.Abstraction;

namespace Tracker.Services;

public sealed class JwtAuthStateProvider : AuthenticationStateProvider
{
    private readonly IAuthStorage _storage;

    public JwtAuthStateProvider(IAuthStorage storage)
        => _storage = storage;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var authSession = await _storage.GetAsync();
        if (authSession is null)
        {
            return Anonymous();
        }

        var claims = new JwtSecurityTokenHandler()
            .ReadJwtToken(authSession.AccessToken)
            .Claims;

        var identity = new ClaimsIdentity(claims, "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    private static AuthenticationState Anonymous()
        => new(new ClaimsPrincipal(new ClaimsIdentity()));
}

