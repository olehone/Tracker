using Microsoft.AspNetCore.Components.Authorization;
using Tracker.Services.Abstraction;

namespace Tracker.Services.Auth;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IAuthService _authService;

    public CustomAuthStateProvider(IAuthService authService)
    {
        _authService = authService;
        _authService.AuthStateChanged += Notify;
    }

    public void Notify()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var principal = await _authService.GetPrincipalAsync();
        return new AuthenticationState(principal);
    }
}