using Microsoft.AspNetCore.Components;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Pages;

public partial class Call : IAsyncDisposable
{
    [Parameter]
    public Guid CallId { get; set; }

    [Inject] private AppState AppState { get; set; } = null!;
    [Inject] private CallState CallState { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

    private bool _disposed = false;
    private string? _expandedVideoId;

    private string? ExpandedVideoId
    {
        get => _expandedVideoId;
        set
        {
            _expandedVideoId = value;
            StateHasChanged();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        if (CallState.IsInCall)
        {
            if (CallState.Call.Id != CallId)
            {
                await CallState.ConnectToCallAsync(CallId);
            }

            return;
        }

        CallState.OnChange += OnCallStateChanged;
        CallState.OnLeaveCall += LeavePage;

        await CallState.InitializeAsync();
        await CallState.ConnectToCallAsync(CallId);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await CallState.AttachStreamsAsync();
    }

    private async Task ExpandVideo(string videoId)
    {
        ExpandedVideoId = videoId;
        StateHasChanged();
        await Task.Yield();
        await CallState.AttachStreamsAsync();
    }

    private async Task CollapseVideo()
    {
        ExpandedVideoId = null;
        StateHasChanged();
        await Task.Yield();
        await CallState.AttachStreamsAsync();
    }

    private void OnCallStateChanged()
    {
        if (_disposed)
        {
            return;
        }

        InvokeAsync(StateHasChanged);
    }

    public Task LeaveCall()
    {
        return CallState.LeaveAsync();
    }

    public async Task LeavePage()
    {
        if (_disposed)
        {
            return;
        }

        Nav.NavigateTo("/");
    }

    public async ValueTask DisposeAsync()
    {
        _disposed = true;
        CallState.OnChange -= OnCallStateChanged;
        CallState.OnLeaveCall -= LeavePage;
    }
}