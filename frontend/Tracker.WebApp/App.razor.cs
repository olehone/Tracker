using Tracker.Services.Abstraction;
using Tracker.Services.States;
using Microsoft.AspNetCore.Components;

public partial class App : ComponentBase
{
    [Inject] private AppState AppState { get; set; } = default!;
    [Inject] private IAuthService AuthService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var user = await AuthService.GetCurrentUserAsync();
        if (user != null)
        {
            AppState.CurrentUser = user;
        }
    }
}