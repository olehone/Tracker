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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await AttachAllStreamsAsync();
    }

    public async Task OnStreamReady(string streamType, string? userId = null)
    {
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

    private async Task AttachAllStreamsAsync()
    {
        await AttachStreamAsync("local_video", "webcam", null);

        if (CallState.IsSharingScreen)
        {
            await AttachStreamAsync("local_screen", "screen", null);
        }

        foreach (var uid in CallState.RemoteUsers)
        {
            await AttachStreamAsync($"video-{uid}", "remote-cam", uid);
        }

        foreach (var uid in CallState.RemoteScreenUsers)
        {
            await AttachStreamAsync($"screen-{uid}", "remote-screen", uid);
        }

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

    private async Task ExpandVideo(string videoId)
    {
        ExpandedVideoId = videoId;
        StateHasChanged();
        await Task.Yield();
        var (st, uid) = ElementIdToStreamType(videoId);
        await AttachStreamAsync(videoId, st, uid);
    }

    private void CollapseVideo()
    {
        ExpandedVideoId = null;
        StateHasChanged();
    }

    private static (string streamType, string? userId) ElementIdToStreamType(string elementId)
    {
        if (elementId == "local_video")
        {
            return ("webcam", null);
        }

        if (elementId == "local_screen")
        {
            return ("screen", null);
        }

        if (elementId.StartsWith("video-"))
        {
            return ("remote-cam", elementId["video-".Length..]);
        }

        if (elementId.StartsWith("screen-"))
        {
            return ("remote-screen", elementId["screen-".Length..]);
        }

        throw new InvalidOperationException($"Unknown element id: {elementId}");
    }

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