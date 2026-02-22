using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Tracker.Services;
using Tracker.Services.Abstraction;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Layout;

public partial class MainLayout : IDisposable
{
    private bool _isDarkMode;
    private bool _isDrawerOpen = true;
    private bool _isParticipantDrawerOpen = false;
    private HashSet<string> _expandedInDrawer = new();
    private System.Threading.Timer? _durationTimer;

    [CascadingParameter]
    AppState AppState { get; set; } = null!;

    [Inject] CallState CallState { get; set; } = null!;
    [Inject] IAuthService AuthService { get; set; } = null!;
    [Inject] NavigationManager Nav { get; set; } = null!;
    [Inject] IJSRuntime JS { get; set; } = null!;

    private bool IsOnCallPage => Nav.Uri == Nav.BaseUri || Nav.Uri.TrimEnd('/') == Nav.BaseUri.TrimEnd('/');

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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // Attach local video to the drawer thumbnail after each render
        if (CallState.IsInCall && !IsOnCallPage && _isParticipantDrawerOpen)
        {
            await JS.InvokeVoidAsync("attachStreamToElement", "drawer_local_video", "local");

            foreach (var userId in _expandedInDrawer)
                await JS.InvokeVoidAsync("attachStreamToElement", $"drawer_video-{userId}", userId);
        }
    }

    private void OnCallStateChanged()
    {
        // Close participant drawer if call ended
        if (!CallState.IsInCall)
        {
            _isParticipantDrawerOpen = false;
            _expandedInDrawer.Clear();
        }

        InvokeAsync(StateHasChanged);
    }

    private void ExpandInDrawer(string userId)
    {
        _expandedInDrawer.Add(userId);
        StateHasChanged();
    }

    private void ToggleParticipantDrawer()
    {
        _isParticipantDrawerOpen = !_isParticipantDrawerOpen;
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
    private void GoToCall() => Nav.NavigateTo("/");

    private void StateHasChangedHandler() => InvokeAsync(StateHasChanged);

    public void Dispose()
    {
        AppState.OnUserChange -= StateHasChangedHandler;
        CallState.OnChange -= OnCallStateChanged;
        _durationTimer?.Dispose();
    }
}
