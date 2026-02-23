using Microsoft.JSInterop;
using Tracker.Services.Abstraction.Realtime;
using Tracker.Services.Abstraction.Realtime.Events;

namespace Tracker.WebApp.States;

public record PeerState(bool Audio, bool Video, bool Screen);
public record CallMetadata(int ParticipantCount, DateTimeOffset? StartedAt);
public record MediaDevice(string DeviceId, string Label);

public class CallState(ICallRealtimeService service, AppState appState, IJSRuntime js) : IAsyncDisposable
{
    private DotNetObjectReference<CallState>? _objRef;

    // -------------------------------------------------------------------------
    // Observable state
    // -------------------------------------------------------------------------

    public event Action? OnChange;

    public bool IsInCall { get; private set; } = false;

    public Guid CallId { get; private set; } = Guid.Parse("29063d2a-7bfb-4384-84b7-0f8625677b0b");

    public CallMetadata? Metadata { get; private set; }
    public DateTimeOffset? CallStartedAt { get; private set; }

    public List<string> RemoteUsers { get; private set; } = new();
    public HashSet<string> RemoteScreenUsers { get; private set; } = new();
    public Dictionary<string, PeerState> PeerStates { get; private set; } = new();

    public bool IsMuted { get; private set; } = false;
    public bool IsVideoEnabled { get; private set; } = true;
    public bool IsSharingScreen { get; private set; } = false;

    public List<MediaDevice> AudioDevices { get; private set; } = new();
    public List<MediaDevice> VideoDevices { get; private set; } = new();
    public string? SelectedAudioDeviceId { get; private set; }
    public string? SelectedVideoDeviceId { get; private set; }

    public string? ExpandedUserId { get; private set; }

    private string MyId => appState.MyId.ToString();
    private bool _connecting = false;

    // -------------------------------------------------------------------------
    // Constructor
    // -------------------------------------------------------------------------

    // Called once from the Call page. Safe to call multiple times.
    public async Task InitializeAsync()
    {
        if (_objRef != null)
            return;
        _objRef = DotNetObjectReference.Create(this);
        await js.InvokeVoidAsync("registerDotNetInstance", _objRef);
    }

    // -------------------------------------------------------------------------
    // Stream attachment — called from OnAfterRenderAsync so DOM is guaranteed ready
    // -------------------------------------------------------------------------

    public async Task AttachStreamsAsync()
    {
        // Always re-attach local video — survives DOM replacement on state change
        await js.InvokeVoidAsync("getLocalStream");
        await js.InvokeVoidAsync("attachLocalStream");

        if (IsInCall)
        {
            foreach (var userId in RemoteUsers)
                await js.InvokeVoidAsync("attachRemoteStream", userId);

            foreach (var userId in RemoteScreenUsers)
                await js.InvokeVoidAsync("attachRemoteScreenStream", userId);

            if (IsSharingScreen)
                await js.InvokeVoidAsync("attachLocalScreenStream");
        }
    }

    // -------------------------------------------------------------------------
    // Peek (lobby)
    // -------------------------------------------------------------------------

    //public async Task StartPeekAsync(string myUserId)
    //{
    //    if (_connecting || IsInCall)
    //        return;
    //    _connecting = true;
    //    IsPreviewing = true;
    //    Service.OnCallMetadataUpdated += HandleCallMetadataUpdated;

    //    await Service.PeekAsync(CallId);
    //    await JS.InvokeVoidAsync("startLocalPreview");
    //    await LoadDevicesAsync();

    //    _connecting = false;
    //    Notify();
    //}

    //private void HandleCallMetadataUpdated(CallMetadataEvent evt)
    //{
    //    Metadata = new CallMetadata(evt.ParticipantCount, evt.StartedAt);
    //    Notify();
    //}

    // -------------------------------------------------------------------------
    // Join
    // -------------------------------------------------------------------------

    public async Task JoinAsync()
    {
        if(appState.IsUnauthenticated || IsInCall)
        {
            return;
        }
        IsInCall = true;
        CallStartedAt = DateTimeOffset.UtcNow;
        Notify();

        service.OnUserListUpdated += HandleUserListUpdated;
        service.OnVideoOffer += HandleVideoOffer;
        service.OnVideoAnswer += HandleVideoAnswer;
        service.OnIceCandidate += HandleIceCandidate;
        service.OnHangUp += HandleHangUp;

        // ConnectAsync invokes Join on the hub — flips status peeking → active
        await service.ConnectAsync(CallId);
    }

    // -------------------------------------------------------------------------
    // Hang up — tears down WebRTC but returns to preview lobby
    // -------------------------------------------------------------------------

    public async Task HangUpAsync()
    {
        await js.InvokeVoidAsync("hangUpAll", new { keepLocalStream = true });

        service.OnUserListUpdated -= HandleUserListUpdated;
        service.OnVideoOffer -= HandleVideoOffer;
        service.OnVideoAnswer -= HandleVideoAnswer;
        service.OnIceCandidate -= HandleIceCandidate;
        service.OnHangUp -= HandleHangUp;

        await service.DisconnectAsync();

        RemoteUsers.Clear();
        RemoteScreenUsers.Clear();
        PeerStates.Clear();
        IsMuted = false;
        IsVideoEnabled = true;
        IsSharingScreen = false;
        IsInCall = false;
        ExpandedUserId = null;
        CallStartedAt = null;

        //// Re-enter peek on same connection
        //IsPreviewing = true;
        //_connecting = false;
        //Service.OnCallMetadataUpdated += HandleCallMetadataUpdated;
        //await Service.PeekAsync(CallId);

        Notify();
    }

    // -------------------------------------------------------------------------
    // SignalR handlers
    // -------------------------------------------------------------------------

    private async void HandleUserListUpdated(UserListUpdatedEvent evt)
    {
        var others = evt.UserIds
            .Where(id => id != MyId)
            .OrderBy(id => id)
            .ToList();

        foreach (var userId in others)
        {
            if (!RemoteUsers.Contains(userId))
                await js.InvokeVoidAsync("initiateCall", userId, MyId);
        }
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
        if (ExpandedUserId == evt.FromUserId)
            ExpandedUserId = null;
        Notify();
    }

    // -------------------------------------------------------------------------
    // JS-invokable callbacks
    // -------------------------------------------------------------------------

    [JSInvokable]
    public void OnRemoteTrack(string userId)
    {
        if (!RemoteUsers.Contains(userId))
        {
            var i = RemoteUsers.BinarySearch(userId, StringComparer.Ordinal);
            RemoteUsers.Insert(i < 0 ? ~i : i, userId);
        }
        Notify();
    }

    [JSInvokable]
    public void OnRemoteScreenTrack(string userId)
    {
        RemoteScreenUsers.Add(userId);
        Notify();
    }

    [JSInvokable]
    public void OnPeerDisconnected(string userId)
    {
        RemoteUsers.Remove(userId);
        RemoteScreenUsers.Remove(userId);
        PeerStates.Remove(userId);
        if (ExpandedUserId == userId)
            ExpandedUserId = null;
        Notify();
    }

    [JSInvokable]
    public void OnPeerStateChanged(string userId, bool audio, bool video, bool screen)
    {
        PeerStates[userId] = new PeerState(audio, video, screen);
        if (!screen)
            RemoteScreenUsers.Remove(userId);
        Notify();
    }

    [JSInvokable]
    public void OnLocalScreenStopped()
    {
        IsSharingScreen = false;
        Notify();
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

    // -------------------------------------------------------------------------
    // Controls
    // -------------------------------------------------------------------------

    public async Task ToggleMuteAsync()
    {
        IsMuted = !IsMuted;
        await js.InvokeVoidAsync("setMuted", IsMuted);
        Notify();
    }

    public async Task ToggleVideoAsync()
    {
        IsVideoEnabled = !IsVideoEnabled;
        await js.InvokeVoidAsync("setVideoEnabled", IsVideoEnabled);
        Notify();
    }

    public async Task ToggleScreenShareAsync()
    {
        if (IsSharingScreen)
        {
            await js.InvokeVoidAsync("stopScreenShare");
            IsSharingScreen = false;
        }
        else
        {
            var started = await js.InvokeAsync<bool>("startScreenShare");
            if (started)
            {
                IsSharingScreen = true;
                await js.InvokeVoidAsync("attachLocalScreenStream");
            }
        }
        Notify();
    }

    public void SetExpandedUser(string? userId)
    {
        ExpandedUserId = userId;
        Notify();
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private void Notify() => OnChange?.Invoke();

    public async ValueTask DisposeAsync()
    {
        //Service.OnCallMetadataUpdated -= HandleCallMetadataUpdated;
        service.OnUserListUpdated -= HandleUserListUpdated;
        service.OnVideoOffer -= HandleVideoOffer;
        service.OnVideoAnswer -= HandleVideoAnswer;
        service.OnIceCandidate -= HandleIceCandidate;
        service.OnHangUp -= HandleHangUp;

        if (IsInCall)
            await js.InvokeVoidAsync("hangUpAll", new { keepLocalStream = false });

        _objRef?.Dispose();
        await service.DisconnectAsync();
    }

    private record DeviceEnumerationResult(List<DeviceInfo> AudioDevices, List<DeviceInfo> VideoDevices);
    private record DeviceInfo(string DeviceId, string Label);
}
