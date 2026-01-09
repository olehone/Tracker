using Microsoft.AspNetCore.Components;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Layout;

public partial class MainLayout : IDisposable
{
    private bool _isDarkMode;
    private bool _isDrawerOpen = true;

    [CascadingParameter]
    private AppState AppState { get; set; } = null!;

    [Inject] private NavigationManager Nav { get; set; } = null!;
    protected override void OnInitialized()
    {
        AppState.OnChange += StateHasChanged;
    }

    void IDisposable.Dispose()
    {
        AppState.OnChange -= StateHasChanged;
    }
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

}