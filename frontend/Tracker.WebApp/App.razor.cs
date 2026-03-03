using Microsoft.AspNetCore.Components;
using Tracker.Services.Abstraction;
using Tracker.WebApp.States;

namespace Tracker.WebApp;

public partial class App : IDisposable
{
    [Inject] private AppState AppState { get; set; } = null!;
    [Inject] private IAuthService AuthService { get; set; } = null!;
    [Inject] private IUserService UserService { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        AppState.OnChange += OnStateChanged;
        AuthService.AuthStateChanged += OnAuthStateChanged;

        await AppState.ReloadAsync();
    }

    private async Task OnStateChanged()
    {
        await InvokeAsync(StateHasChanged);
    }

    private async void OnAuthStateChanged()
    {
        await InvokeAsync(AppState.ReloadAsync);
    }

    public void Dispose()
    {
        AuthService.AuthStateChanged -= OnAuthStateChanged;
    }

    public void RedirectToLogin()
    {
        Nav.NavigateTo("/login");
    }
}