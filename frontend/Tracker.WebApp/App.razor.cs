using Microsoft.AspNetCore.Components;
using Tracker.Services.Abstraction;
using Tracker.Services.States;

namespace Tracker.WebApp;
public partial class App
{
    [Inject] private AppState AppState { get; set; } = default!;
    [Inject] private IAuthService AuthService { get; set; } = null!;
    [Inject] private IUserService UserService { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {

        var principal = await AuthService.GetPrincipalAsync();

        if (principal.Identity?.IsAuthenticated == true)
        {
            AppState.CurrentUser = await UserService.GetCurrentUserAsync();
        }
    }
}