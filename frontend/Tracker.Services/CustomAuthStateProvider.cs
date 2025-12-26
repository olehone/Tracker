using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Tracker.Services.Abstraction;

namespace Tracker.Services;

public class CustomAuthStateProvider
    : AuthenticationStateProvider, IAuthStateNotifier
{
    private readonly IAuthService _authService;
    public CustomAuthStateProvider(IAuthService authService)
    {
        _authService = authService;
        //_authService.OnLogin += NotifyUserAuthentication;
        //_authService.OnLogout += NotifyUserLogout;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var principal = await _authService.GetPrincipalAsync();
        return new AuthenticationState(principal);
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
