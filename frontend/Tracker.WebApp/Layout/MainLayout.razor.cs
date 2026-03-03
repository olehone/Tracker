using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Services.Abstraction;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Layout;

public partial class MainLayout : IDisposable
{
    private bool _isDarkMode;
    private bool _isDrawerOpen = true;
    private bool _isFaqOpen = false;

    [CascadingParameter]
    private AppState AppState { get; set; } = null!;

    [Inject] private CallState CallState { get; set; } = null!;
    [Inject] private IAuthService AuthService { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

    protected override void OnInitialized()
    {
        AppState.OnChange += StateHasChangedHandler;
        CallState.OnChange += OnCallStateChanged;

    }

    private Color GetAppBarColor()
    {
        if (CallState.IsInCall)
        {
            return Color.Tertiary;
        }

        return Color.Primary;
    }

    private void OnCallStateChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    private async Task Logout()
    {
        await AuthService.LogoutAsync();
        await AppState.ReloadAsync();
        Nav.NavigateTo("/");
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

    private void GoToCall()
    {
        Nav.NavigateTo("/call");
    }

    private Task StateHasChangedHandler()
    {
        return InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        AppState.OnChange -= StateHasChangedHandler;
        CallState.OnChange -= OnCallStateChanged;
    }
}
