using Microsoft.JSInterop;
using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Realtime;
using Tracker.Services.Abstraction.Realtime.Events.Calls;

namespace Tracker.WebApp.States;

public class CallState(ICallService callService,
    ICallRealtimeService realtimeService,
    AppState appState,
    IJSRuntime js) : IAsyncDisposable
{
    private DotNetObjectReference<CallState>? _objRef;
    private CallDto? _currentCall;

    public static string LocalVideoElementId => "local_video";
    public static string LocalScreenElementId => "local_screen";
    public static string RemoteCamElementId(string userId) => $"video-{userId}";
    public static string RemoteScreenElementId(string userId) => $"screen-{userId}";

    public event Action? OnChange;
    public event Action? OnLeaveCall;

    public CallDto Call => _currentCall!;
    public bool IsInCall { get; private set; } = false;
    public DateTimeOffset? CallStartedAt => Call?.StartedAt;

    public List<string> RemoteUsers { get; private set; } = [];
    public HashSet<string> RemoteScreenUsers { get; private set; } = [];
    public Dictionary<string, PeerState> PeerStates { get; private set; } = [];

    public bool IsMuted { get; private set; } = false;
    public bool IsVideoEnabled { get; private set; } = true;
    public bool IsSharingScreen { get; private set; } = false;
    public string? ScreenStreamId { get; private set; } = null;

    public string? ExpandedUserId { get; private set; }

    private string MyId => appState.MyId.ToString();


    public async Task InitializeAsync()
    {
        if (_objRef != null)
        {
            return;
        }

        _objRef = DotNetObjectReference.Create(this);
        await js.InvokeVoidAsync("registerDotNetInstance", _objRef);
    }

    

    public async Task ConnectToCallAsync(Guid callId)
    {
        if (IsInCall)
        {
            await LeaveAsync();
        }
        var result = await callService.GetByIdAsync(callId);
        if (result.IsSuccess)
        {
            _currentCall = result.Value;
            await JoinAsync();
        }
    }

    public async Task AttachStreamAsync(string elementId, string type, string? userId)
    {
        await js.InvokeVoidAsync("attachStream", elementId, type, userId);
    }

    public async Task AttachStreamsAsync()
    {
        await js.InvokeVoidAsync("getLocalStream");
        await AttachStreamAsync(LocalVideoElementId, "webcam", null);
        if (!IsInCall)
        {
            return;
        }

        foreach (var userId in RemoteUsers)
        {
            await AttachStreamAsync(RemoteCamElementId(userId), "remote-cam", userId);
        }

        foreach (var userId in RemoteScreenUsers)
        {
            await AttachStreamAsync(RemoteScreenElementId(userId), "remote-screen", userId);
        }

        if (IsSharingScreen)
        {
            await AttachStreamAsync(LocalScreenElementId, "screen", null);
        }
    }

    public async Task JoinAsync()
    {
        if (appState.IsUnauthenticated || IsInCall)
        {
            return;
        }

        IsInCall = true;
        Notify();

        realtimeService.OnCallEnded += HandleCallEnded;
        realtimeService.OnUserJoined += HandleUserJoined;
        realtimeService.OnUserLeaved += HandleUserLeaved;

        realtimeService.OnVideoOffer += HandleVideoOffer;
        realtimeService.OnVideoAnswer += HandleVideoAnswer;
        realtimeService.OnIceCandidate += HandleIceCandidate;

        await realtimeService.ConnectAsync(Call.Id);
    }

    public async Task LeaveAsync()
    {
        await realtimeService.LeaveAsync(Call.Id);
        await realtimeService.DisconnectAsync();
        HandleCallEnded();
    }

    private async void HandleCallEnded()
    {
        UnsubscribeSignalR();
        RemoteUsers.Clear();
        RemoteScreenUsers.Clear();
        PeerStates.Clear();
        IsMuted = false;
        IsVideoEnabled = true;
        IsSharingScreen = false;
        ScreenStreamId = null;
        IsInCall = false;
        _currentCall = null;
        OnLeaveCall?.Invoke();
        await js.InvokeVoidAsync("closeStreams");
        Notify();
    }

    private async void HandleUserJoined(UserJoinedEvent evt)
    {
        var userId = evt.User.Id.ToString();

        var i = RemoteUsers.BinarySearch(userId, StringComparer.Ordinal);
        RemoteUsers.Insert(i < 0 ? ~i : i, userId);

        PeerStates.TryAdd(userId, new PeerState(false, false, false));

        await js.InvokeVoidAsync("initiateCall", userId, MyId);

        Notify();
    }

    private async void HandleUserLeaved(UserLeavedEvent evt)
    {
        var userId = evt.UserId.ToString();

        RemoteUsers.Remove(userId);
        RemoteScreenUsers.Remove(userId);
        PeerStates.Remove(userId);
        await js.InvokeVoidAsync("receiveLeave", userId);

        Notify();
    }

    private void HandleVideoOffer(VideoOfferEvent evt)
        => js.InvokeVoidAsync("receiveVideoOffer", evt.FromUserId, evt.Sdp);

    private void HandleVideoAnswer(VideoAnswerEvent evt)
        => js.InvokeVoidAsync("receiveVideoAnswer", evt.FromUserId, evt.Sdp);

    private void HandleIceCandidate(IceCandidateEvent evt)
        => js.InvokeVoidAsync("receiveIceCandidate", evt.FromUserId, evt.CandidateJson);


    [JSInvokable]
    public object GetLocalState() => new
    {
        audio = !IsMuted,
        video = IsVideoEnabled,
        screen = IsSharingScreen,
        screenStreamId = ScreenStreamId,
    };

    [JSInvokable]
    public async Task OnRemoteTrack(string userId)
    {
        if (!RemoteUsers.Contains(userId))
        {
            var i = RemoteUsers.BinarySearch(userId, StringComparer.Ordinal);
            RemoteUsers.Insert(i < 0 ? ~i : i, userId);
        }
        Notify();
        await Task.Yield();
        await AttachStreamAsync(RemoteCamElementId(userId), "remote-cam", userId);
    }

    [JSInvokable]
    public async Task OnRemoteScreenTrack(string userId)
    {
        RemoteScreenUsers.Add(userId);
        Notify();
        await Task.Yield();
        await AttachStreamAsync(RemoteScreenElementId(userId), "remote-screen", userId);
    }

    [JSInvokable]
    public void OnPeerDisconnected(string userId)
    {
        RemoteUsers.Remove(userId);
        RemoteScreenUsers.Remove(userId);
        PeerStates.Remove(userId);
        Notify();
    }

    [JSInvokable]
    public void OnPeerStateChanged(string userId, bool audio, bool video, bool screen)
    {
        PeerStates[userId] = new PeerState(audio, video, screen);
        if (!screen)
        {
            RemoteScreenUsers.Remove(userId);
        }

        Notify();
    }

    [JSInvokable]
    public void OnLocalScreenStopped()
    {
        IsSharingScreen = false;
        ScreenStreamId = null;
        Notify();
        _ = BroadcastStateAsync();
    }

    [JSInvokable]
    public Task SendVideoOffer(string targetUserId, string sdp)
        => realtimeService.SendVideoOffer(Call.Id, targetUserId, sdp);

    [JSInvokable]
    public Task SendVideoAnswer(string targetUserId, string sdp)
        => realtimeService.SendVideoAnswer(Call.Id, targetUserId, sdp);

    [JSInvokable]
    public Task SendIceCandidate(string targetUserId, string candidateJson)
        => realtimeService.SendIceCandidate(Call.Id, targetUserId, candidateJson);

    public async Task ToggleMuteAsync()
    {
        IsMuted = !IsMuted;
        await js.InvokeVoidAsync("setMuted", IsMuted);
        await BroadcastStateAsync();
        Notify();
    }

    public async Task ToggleVideoAsync()
    {
        IsVideoEnabled = !IsVideoEnabled;
        await js.InvokeVoidAsync("setVideoEnabled", IsVideoEnabled);
        await BroadcastStateAsync();
        Notify();
    }

    public async Task ToggleScreenShareAsync()
    {
        if (IsSharingScreen)
        {
            await js.InvokeVoidAsync("stopScreenShare");
        }
        else
        {
            var streamId = await js.InvokeAsync<string?>("startScreenShare");
            if (streamId is not null)
            {
                IsSharingScreen = true;
                ScreenStreamId = streamId;
                Notify();
                await Task.Yield();
                await js.InvokeVoidAsync("attachStream", LocalScreenElementId, "screen", null);
                await BroadcastStateAsync();
            }
        }
    }

    private Task BroadcastStateAsync()
        => js.InvokeVoidAsync("broadcastState", !IsMuted, IsVideoEnabled, IsSharingScreen, ScreenStreamId).AsTask();

    private void UnsubscribeSignalR()
    {
        realtimeService.OnCallEnded -= HandleCallEnded;
        realtimeService.OnUserJoined -= HandleUserJoined;
        realtimeService.OnUserLeaved -= HandleUserLeaved;
        realtimeService.OnVideoOffer -= HandleVideoOffer;
        realtimeService.OnVideoAnswer -= HandleVideoAnswer;
        realtimeService.OnIceCandidate -= HandleIceCandidate;
    }

    private void Notify() => OnChange?.Invoke();

    public async ValueTask DisposeAsync()
    {
        UnsubscribeSignalR();
        if (IsInCall)
        {
            await js.InvokeVoidAsync("closeStreams", new { keepLocalStream = false });
        }

        _objRef?.Dispose();
        await realtimeService.DisconnectAsync();
    }
}