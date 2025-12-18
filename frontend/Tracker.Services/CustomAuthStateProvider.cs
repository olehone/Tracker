using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Tracker.Services.Abstraction;

namespace Tracker.Services;

public class CustomAuthStateProvider(IAuthStorage storage) 
    : AuthenticationStateProvider, IAuthStateNotifier
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var tokensDto = await storage.GetAsync();
        if (tokensDto is null)
        {
            return Anonymous();
        }

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(tokensDto.AccessToken);
        var claims = new List<Claim>(jwt.Claims);
        var identity = new ClaimsIdentity(claims, "jwt");

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void NotifyUserAuthentication()
    {
        var authState = GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous()));
    }

    private static AuthenticationState Anonymous()
        => new(new ClaimsPrincipal(new ClaimsIdentity()));
}
