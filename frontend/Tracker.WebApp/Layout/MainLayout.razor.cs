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
        AppState.OnUserChange += StateHasChangedHandler;
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

    private void GoToHome()
    {
        Nav.NavigateTo("/");
    }

    private void StateHasChangedHandler()
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        AppState.OnUserChange -= StateHasChangedHandler;
    }
}