using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Tracker.Services.Abstraction;
using Tracker.WebApp.States;

namespace Tracker.WebApp;

public partial class App
{
    [Inject]
    private AppState AppState { get; set; } = default!;

    [Inject]
    private IAuthService AuthService { get; set; } = default!;

    [Inject]
    private IUserService UserService { get; set; } = default!;

    [Inject]
    private NavigationManager Nav { get; set; } = default!;

    [Inject]
    private IWebAssemblyHostEnvironment Env { get; set; } = default!;

    private void RedirectToLogin()
    {
        Nav.NavigateTo($"/login", forceLoad: false);
    }

    protected override async Task OnInitializedAsync()
    {
        var principal = await AuthService.GetPrincipalAsync();

        if (principal.Identity?.IsAuthenticated == true)
        {
            AppState.CurrentUser = await UserService.GetCurrentUserAsync();
        }
    }
}