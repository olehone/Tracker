using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Tracker.Services.Abstraction;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Layout;

public partial class MainLayout : IDisposable
{
    private bool _isDarkMode;
    private bool _isDrawerOpen = true;
    private System.Threading.Timer? _durationTimer;

    [CascadingParameter]
    AppState AppState { get; set; } = null!;

    [Inject] CallState CallState { get; set; } = null!;
    [Inject] IAuthService AuthService { get; set; } = null!;
    [Inject] NavigationManager Nav { get; set; } = null!;
    [Inject] IJSRuntime JS { get; set; } = null!;

    private string CallDuration
    {
        get
        {
            if (CallState.CallStartedAt is null)
                return "0:00";
            var elapsed = DateTimeOffset.UtcNow - CallState.CallStartedAt.Value;
            return elapsed.TotalHours >= 1
                ? $"{(int)elapsed.TotalHours}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}"
                : $"{elapsed.Minutes}:{elapsed.Seconds:D2}";
        }
    }

    protected override void OnInitialized()
    {
        AppState.OnUserChange += StateHasChangedHandler;
        CallState.OnChange += OnCallStateChanged;

        // Tick every second to update the call duration display
        _durationTimer = new System.Threading.Timer(_ =>
            InvokeAsync(StateHasChanged), null,
            TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    private void OnCallStateChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    private async Task Logout()
    {
        await AuthService.LogoutAsync();
        AppState.Clear();
        Nav.NavigateTo("/");
    }

    private void DrawerToggle() => _isDrawerOpen = !_isDrawerOpen;
    private void GoToLogin() => Nav.NavigateTo("/login");
    private void GoToRegister() => Nav.NavigateTo("/register");
    private void GoToHome() => Nav.NavigateTo("/");
    private void GoToCall() => Nav.NavigateTo("/call");

    private void StateHasChangedHandler() => InvokeAsync(StateHasChanged);

    public void Dispose()
    {
        AppState.OnUserChange -= StateHasChangedHandler;
        CallState.OnChange -= OnCallStateChanged;
        _durationTimer?.Dispose();
    }
}
