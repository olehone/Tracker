using Microsoft.AspNetCore.Components;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.Layout;
public partial class MainLayout
{
    private bool _isDarkMode = false;
    private bool _isDrawerOpen = true;

    [Inject] NavigationManager Nav { get; set; } = default!;
    [Inject] IAuthService AuthService { get; set; } = default!;
    [Inject] IAuthStateNotifier AuthNotifier { get; set; } = default!;
    void DrawerToggle()
    {
        _isDrawerOpen = !_isDrawerOpen;
    }
    private void GoToLogin() => Nav.NavigateTo("/login");
    private void GoToRegister() => Nav.NavigateTo("/register");
    private async Task Logout()
    {
        await AuthService.LogoutAsync();
        AuthNotifier.NotifyUserLogout();
        Nav.NavigateTo("/");
    }
}