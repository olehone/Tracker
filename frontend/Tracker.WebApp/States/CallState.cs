using Microsoft.JSInterop;
using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction.Realtime;
using Tracker.Services.Abstraction.Realtime.Events;

namespace Tracker.WebApp.States;

public class CallState(ICallRealtimeService service, AppState appState, IJSRuntime js) : IAsyncDisposable
{
    private DotNetObjectReference<CallState>? _objRef;

    public static string LocalVideoElementId => "local_video";
    public static string LocalScreenElementId => "local_screen";
    public static string RemoteCamElementId(string userId) => $"video-{userId}";
    public static string RemoteScreenElementId(string userId) => $"screen-{userId}";

    public event Action? OnChange;
    public event Action? OnLeaveCall;

    public bool IsInCall { get; private set; } = false;
    // Placeholder
    public Guid CallId { get; private set; } = Guid.Parse("29063d2a-7bfb-4384-84b7-0f8625677b0b");

    public CallMetadata? Metadata { get; private set; }
    public DateTimeOffset? CallStartedAt => Metadata?.StartedAt;

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

        service.OnUserListUpdated += HandleUserListUpdated;
        service.OnVideoOffer += HandleVideoOffer;
        service.OnVideoAnswer += HandleVideoAnswer;
        service.OnIceCandidate += HandleIceCandidate;
        service.OnHangUp += HandleHangUp;
        service.OnCallMetadataUpdated += HandleMetadata;

        await service.ConnectAsync(CallId);
    }

    public async Task HangUpAsync()
    {
        await js.InvokeVoidAsync("hangUpAll", new { keepLocalStream = false });

        UnsubscribeSignalR();
        await service.DisconnectAsync();

        RemoteUsers.Clear();
        RemoteScreenUsers.Clear();
        PeerStates.Clear();
        IsMuted = false;
        IsVideoEnabled = true;
        IsSharingScreen = false;
        ScreenStreamId = null;
        IsInCall = false;
        OnLeaveCall?.Invoke();
        Notify();
    }

    private async void HandleUserListUpdated(UserListUpdatedEvent evt)
    {
        var others = evt.UserIds
            .Where(id => id != MyId)
            .OrderBy(id => id)
            .ToList();

        foreach (var userId in others)
        {
            if (!RemoteUsers.Contains(userId))
            {
                var i = RemoteUsers.BinarySearch(userId, StringComparer.Ordinal);
                RemoteUsers.Insert(i < 0 ? ~i : i, userId);

                PeerStates.TryAdd(userId, new PeerState(false, false, false));

                await js.InvokeVoidAsync("initiateCall", userId, MyId);
            }
        }

        Notify();
    }

    private void HandleVideoOffer(VideoOfferEvent evt)
        => js.InvokeVoidAsync("receiveVideoOffer", evt.FromUserId, evt.Sdp);

    private void HandleVideoAnswer(VideoAnswerEvent evt)
        => js.InvokeVoidAsync("receiveVideoAnswer", evt.FromUserId, evt.Sdp);

    private void HandleIceCandidate(IceCandidateEvent evt)
        => js.InvokeVoidAsync("receiveIceCandidate", evt.FromUserId, evt.CandidateJson);

    private void HandleHangUp(HangUpEvent evt)
    {
        RemoteUsers.Remove(evt.FromUserId);
        RemoteScreenUsers.Remove(evt.FromUserId);
        PeerStates.Remove(evt.FromUserId);
        js.InvokeVoidAsync("receiveHangUp", evt.FromUserId);
        Notify();
    }

    private void HandleMetadata(CallMetadataEvent evt)
    {
        Metadata = new CallMetadata(evt.ParticipantCount, evt.StartedAt);
    }

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
        => service.SendVideoOffer(CallId, targetUserId, sdp);

    [JSInvokable]
    public Task SendVideoAnswer(string targetUserId, string sdp)
        => service.SendVideoAnswer(CallId, targetUserId, sdp);

    [JSInvokable]
    public Task SendIceCandidate(string targetUserId, string candidateJson)
        => service.SendIceCandidate(CallId, targetUserId, candidateJson);

    [JSInvokable]
    public Task SendHangUp(string targetUserId)
        => service.SendHangUp(CallId, targetUserId);

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
        service.OnUserListUpdated -= HandleUserListUpdated;
        service.OnVideoOffer -= HandleVideoOffer;
        service.OnVideoAnswer -= HandleVideoAnswer;
        service.OnIceCandidate -= HandleIceCandidate;
        service.OnHangUp -= HandleHangUp;
        service.OnCallMetadataUpdated -= HandleMetadata;
    }

    private void Notify() => OnChange?.Invoke();

    public async ValueTask DisposeAsync()
    {
        UnsubscribeSignalR();
        if (IsInCall)
        {
            await js.InvokeVoidAsync("hangUpAll", new { keepLocalStream = false });
        }

        _objRef?.Dispose();
        await service.DisconnectAsync();
    }
}