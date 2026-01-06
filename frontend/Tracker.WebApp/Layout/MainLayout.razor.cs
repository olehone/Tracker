using Microsoft.AspNetCore.Components;
using Tracker.Services.Abstraction;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Layout;

public partial class MainLayout
{
    private bool _isDarkMode;
    private bool _isDrawerOpen = true;

    [CascadingParameter]
    private AppState AppState { get; set; } = null!;

    [Inject]
    private NavigationManager Nav { get; set; } = null!;
    [Inject]
    private IAuthService AuthService { get; set; } = null!;

    private void DrawerToggle()
    {
        _isDrawerOpen = !_isDrawerOpen;
    }

    private void GoToLogin()
    {
        Nav.NavigateTo("/login");
    }

    private void GoToRegister()
    {
        Nav.NavigateTo("/register");
    }

    private async Task Logout()
    {
        await AuthService.LogoutAsync();
        Nav.NavigateTo("/");
    }
}