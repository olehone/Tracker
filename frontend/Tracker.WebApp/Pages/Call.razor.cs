using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Pages;

public partial class Call : IAsyncDisposable
{
    [Inject] AppState AppState{ get; set; } = null!;
    [Inject] CallState CallState { get; set; } = null!;
    [Inject] IJSRuntime JS { get; set; } = null!;
    [Inject] NavigationManager Nav{ get; set; } = null!;


    // ── After every render, re-attach streams to whatever video elements exist ──
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await AttachAllStreamsAsync();
    }

    // ── Called by SignalR / CallStateService whenever a new stream is ready ──
    public async Task OnStreamReady(string streamType, string? userId = null)
    {
        // Derive the element ID the same way the markup does
        string elementId = streamType switch
        {
            "webcam" => "local_video",
            "screen" => "local_screen",
            "remote-cam" => $"video-{userId}",
            "remote-screen" => $"screen-{userId}",
            _ => throw new ArgumentOutOfRangeException(nameof(streamType))
        };

        await AttachStreamAsync(elementId, streamType, userId);
    }

    // ── Attach every known stream to its element ──
    private async Task AttachAllStreamsAsync()
    {
        await AttachStreamAsync("local_video", "webcam", null);

        if (CallState.IsSharingScreen)
            await AttachStreamAsync("local_screen", "screen", null);

        foreach (var uid in CallState.RemoteUsers)
            await AttachStreamAsync($"video-{uid}", "remote-cam", uid);

        foreach (var uid in CallState.RemoteScreenUsers)
            await AttachStreamAsync($"screen-{uid}", "remote-screen", uid);

        // Also re-attach expanded video if one is open
        if (ExpandedVideoId != null)
        {
            var (st, uid) = ElementIdToStreamType(ExpandedVideoId);
            await AttachStreamAsync(ExpandedVideoId, st, uid);
        }
    }

    private async Task AttachStreamAsync(string elementId, string streamType, string? userId)
    {
        try
        {
            await JS.InvokeVoidAsync("attachStream", elementId, streamType, userId);
        }
        catch (JSException ex)
        {
            Console.Error.WriteLine($"[attachStream] {elementId}: {ex.Message}");
        }
    }

    // ── Expand / collapse helpers ──
    private async Task ExpandVideo(string videoId)
    {
        ExpandedVideoId = videoId;
        StateHasChanged();
        // Give Blazor one tick to render the expanded <video> element
        await Task.Yield();
        var (st, uid) = ElementIdToStreamType(videoId);
        await AttachStreamAsync(videoId, st, uid);
    }

    private void CollapseVideo()
    {
        ExpandedVideoId = null;
        StateHasChanged();
    }

    private bool IsLocalVideo(string id) =>
        id == "local_video" || id == "local_screen";

    /// <summary>
    /// Reverse-maps an element ID back to (streamType, userId) so we can
    /// call attachStream with the right arguments.
    /// </summary>
    private static (string streamType, string? userId) ElementIdToStreamType(string elementId)
    {
        if (elementId == "local_video")
            return ("webcam", null);
        if (elementId == "local_screen")
            return ("screen", null);
        if (elementId.StartsWith("video-"))
            return ("remote-cam", elementId["video-".Length..]);
        if (elementId.StartsWith("screen-"))
            return ("remote-screen", elementId["screen-".Length..]);
        throw new InvalidOperationException($"Unknown element id: {elementId}");
    }
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
        CallState.OnChange += OnCallStateChanged;
        CallState.OnLeaveCall += LeavePage;
        AppState.OnUserChange += OnCallStateChanged;
        await CallState.InitializeAsync();
        await CallState.JoinAsync();
    }

    //protected override async Task OnAfterRenderAsync(bool firstRender)
    //{
    //    if (firstRender)
    //        await CallState.AttachStreamsAsync();
    //}

    private void OnCallStateChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    public Task LeaveCall()
    {
        return CallState.HangUpAsync();
    }

    public void LeavePage()
    {
        Nav.NavigateTo("/");
    }

    public async ValueTask DisposeAsync()
    {
        CallState.OnChange -= OnCallStateChanged;
        AppState.OnUserChange -= OnCallStateChanged;
    }
}