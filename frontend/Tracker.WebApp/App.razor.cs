using Microsoft.AspNetCore.Components;
using Tracker.Services.Abstraction;
using Tracker.WebApp.States;

namespace Tracker.WebApp;

public partial class App : IDisposable
{
    [Inject] AppState AppState { get; set; } = null!;
    [Inject] IAuthService AuthService { get; set; } = null!;
    [Inject] IUserService UserService { get; set; } = null!;
    [Inject] NavigationManager Nav { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        AuthService.AuthStateChanged += OnAuthStateChanged;
        await LoadUserAsync();
    }

    private async void OnAuthStateChanged()
    {
        await InvokeAsync(LoadUserAsync);
    }

    private async Task LoadUserAsync()
    {
        AppState.StartLoading();

        var principal = await AuthService.GetPrincipalAsync();

        if (principal.Identity?.IsAuthenticated is not true)
        {
            AppState.Clear();
            return;
        }

        var result = await UserService.GetCurrentAsync();
        if (result.IsFailure)
        {
            AppState.Clear();
            return;
        }

        AppState.CurrentUser = result.Value;
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