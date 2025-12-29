using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Auth;

namespace Tracker.Services.Auth;

public class CustomAuthStateProvider
    : AuthenticationStateProvider, IAuthStateNotifier
{
    private readonly IAuthService _authService;
    public CustomAuthStateProvider(IAuthService authService)
    {
        _authService = authService;
        _authService.OnLogin = EventCallback.Factory.Create(this, NotifyUserAuthentication);
        _authService.OnLogout = EventCallback.Factory.Create(this, NotifyUserLogout);
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
