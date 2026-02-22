using Microsoft.JSInterop;
using Tracker.Services.Abstraction.Realtime;
using Tracker.Services.Abstraction.Realtime.Events;

namespace Tracker.WebApp.States;

public record PeerState(bool Audio, bool Video, bool Screen);
public record CallMetadata(int ParticipantCount, DateTimeOffset? StartedAt);
public record MediaDevice(string DeviceId, string Label);

public class CallState : IAsyncDisposable
{
    private readonly ICallRealtimeService _callService;
    private readonly IJSRuntime _js;
    private DotNetObjectReference<CallState>? _objRef;

    // -------------------------------------------------------------------------
    // Observable state
    // -------------------------------------------------------------------------

    public event Action? OnChange;

    public bool IsPreviewing { get; private set; } = false;
    public bool IsInCall { get; private set; } = false;
    public bool IsActive => IsPreviewing || IsInCall;

    public Guid CallId { get; private set; } = Guid.Parse("29063d2a-7bfb-4384-84b7-0f8625677b0b");

    public CallMetadata? Metadata { get; private set; }
    public DateTimeOffset? CallStartedAt { get; private set; }

    public List<string> RemoteUsers { get; private set; } = new();
    public HashSet<string> RemoteScreenUsers { get; private set; } = new();
    public Dictionary<string, PeerState> PeerStates { get; private set; } = new();

    public bool IsMuted { get; private set; } = false;
    public bool IsVideoEnabled { get; private set; } = true;
    public bool IsSharingScreen { get; private set; } = false;
    public bool IsSharingLocalScreen { get; private set; } = false;

    public List<MediaDevice> AudioDevices { get; private set; } = new();
    public List<MediaDevice> VideoDevices { get; private set; } = new();
    public string? SelectedAudioDeviceId { get; private set; }
    public string? SelectedVideoDeviceId { get; private set; }

    public string? ExpandedUserId { get; private set; }

    private string? _myUserId;
    private bool _connecting = false;

    // -------------------------------------------------------------------------
    // Constructor
    // -------------------------------------------------------------------------

    public CallState(ICallRealtimeService callService, IJSRuntime js)
    {
        _callService = callService;
        _js = js;
    }

    // Called once from the Call page. Safe to call multiple times.
    public async Task InitializeAsync()
    {
        if (_objRef != null) return;
        _objRef = DotNetObjectReference.Create(this);
        await _js.InvokeVoidAsync("registerDotNetInstance", _objRef);
    }

    // -------------------------------------------------------------------------
    // Stream attachment — called from OnAfterRenderAsync so DOM is guaranteed ready
    // -------------------------------------------------------------------------

    public async Task AttachStreamsAsync()
    {
        // Always re-attach local video — survives DOM replacement on state change
        await _js.InvokeVoidAsync("attachLocalStream");

        if (IsInCall)
        {
            foreach (var userId in RemoteUsers)
                await _js.InvokeVoidAsync("attachRemoteStream", userId);

            foreach (var userId in RemoteScreenUsers)
                await _js.InvokeVoidAsync("attachRemoteScreenStream", userId);

            if (IsSharingLocalScreen)
                await _js.InvokeVoidAsync("attachLocalScreenStream");
        }
    }

    // -------------------------------------------------------------------------
    // Peek (lobby)
    // -------------------------------------------------------------------------

    public async Task StartPeekAsync(string myUserId)
    {
        if (_connecting || IsInCall) return;
        _connecting = true;
        _myUserId = myUserId;

        IsPreviewing = true;
        _callService.OnCallMetadataUpdated += HandleCallMetadataUpdated;

        await _callService.PeekAsync(CallId);
        await _js.InvokeVoidAsync("startLocalPreview");
        await LoadDevicesAsync();

        _connecting = false;
        Notify();
    }

    private void HandleCallMetadataUpdated(CallMetadataEvent evt)
    {
        Metadata = new CallMetadata(evt.ParticipantCount, evt.StartedAt);
        Notify();
    }

    // -------------------------------------------------------------------------
    // Join
    // -------------------------------------------------------------------------

    public async Task JoinAsync()
    {
        if (!IsPreviewing || _myUserId == null) return;

        IsPreviewing = false;
        IsInCall = true;
        CallStartedAt = DateTimeOffset.UtcNow;
        Notify();

        _callService.OnCallMetadataUpdated -= HandleCallMetadataUpdated;
        _callService.OnUserListUpdated += HandleUserListUpdated;
        _callService.OnVideoOffer += HandleVideoOffer;
        _callService.OnVideoAnswer += HandleVideoAnswer;
        _callService.OnIceCandidate += HandleIceCandidate;
        _callService.OnHangUp += HandleHangUp;

        // ConnectAsync invokes Join on the hub — flips status peeking → active
        await _callService.ConnectAsync(CallId);
    }

    // -------------------------------------------------------------------------
    // Hang up — tears down WebRTC but returns to preview lobby
    // -------------------------------------------------------------------------

    public async Task HangUpAsync()
    {
        await _js.InvokeVoidAsync("hangUpAll", new { keepLocalStream = true });

        _callService.OnUserListUpdated -= HandleUserListUpdated;
        _callService.OnVideoOffer -= HandleVideoOffer;
        _callService.OnVideoAnswer -= HandleVideoAnswer;
        _callService.OnIceCandidate -= HandleIceCandidate;
        _callService.OnHangUp -= HandleHangUp;

        await _callService.DisconnectAsync();

        RemoteUsers.Clear();
        RemoteScreenUsers.Clear();
        PeerStates.Clear();
        IsMuted = false;
        IsVideoEnabled = true;
        IsSharingScreen = false;
        IsSharingLocalScreen = false;
        IsInCall = false;
        ExpandedUserId = null;
        CallStartedAt = null;

        // Re-enter peek on same connection
        IsPreviewing = true;
        _connecting = false;
        _callService.OnCallMetadataUpdated += HandleCallMetadataUpdated;
        await _callService.PeekAsync(CallId);

        Notify();
    }

    // -------------------------------------------------------------------------
    // SignalR handlers
    // -------------------------------------------------------------------------

    private async void HandleUserListUpdated(UserListUpdatedEvent evt)
    {
        if (_myUserId == null) return;

        var others = evt.UserIds
            .Where(id => id != _myUserId)
            .OrderBy(id => id)
            .ToList();

        foreach (var userId in others)
        {
            if (!RemoteUsers.Contains(userId))
                await _js.InvokeVoidAsync("initiateCall", userId, _myUserId);
        }
    }

    private void HandleVideoOffer(VideoOfferEvent evt)
        => _js.InvokeVoidAsync("receiveVideoOffer", evt.FromUserId, evt.Sdp);

    private void HandleVideoAnswer(VideoAnswerEvent evt)
        => _js.InvokeVoidAsync("receiveVideoAnswer", evt.FromUserId, evt.Sdp);

    private void HandleIceCandidate(IceCandidateEvent evt)
        => _js.InvokeVoidAsync("receiveIceCandidate", evt.FromUserId, evt.CandidateJson);

    private void HandleHangUp(HangUpEvent evt)
    {
        RemoteUsers.Remove(evt.FromUserId);
        RemoteScreenUsers.Remove(evt.FromUserId);
        PeerStates.Remove(evt.FromUserId);
        _js.InvokeVoidAsync("receiveHangUp", evt.FromUserId);
        if (ExpandedUserId == evt.FromUserId) ExpandedUserId = null;
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
        if (ExpandedUserId == userId) ExpandedUserId = null;
        Notify();
    }

    [JSInvokable]
    public void OnPeerStateChanged(string userId, bool audio, bool video, bool screen)
    {
        PeerStates[userId] = new PeerState(audio, video, screen);
        if (!screen) RemoteScreenUsers.Remove(userId);
        Notify();
    }

    [JSInvokable]
    public void OnLocalScreenStopped()
    {
        IsSharingScreen = false;
        IsSharingLocalScreen = false;
        Notify();
    }

    [JSInvokable]
    public Task SendVideoOffer(string targetUserId, string sdp)
        => _callService.SendVideoOffer(CallId, targetUserId, sdp);

    [JSInvokable]
    public Task SendVideoAnswer(string targetUserId, string sdp)
        => _callService.SendVideoAnswer(CallId, targetUserId, sdp);

    [JSInvokable]
    public Task SendIceCandidate(string targetUserId, string candidateJson)
        => _callService.SendIceCandidate(CallId, targetUserId, candidateJson);

    [JSInvokable]
    public Task SendHangUp(string targetUserId)
        => _callService.SendHangUp(CallId, targetUserId);

    // -------------------------------------------------------------------------
    // Controls
    // -------------------------------------------------------------------------

    public async Task ToggleMuteAsync()
    {
        IsMuted = !IsMuted;
        await _js.InvokeVoidAsync("setMuted", IsMuted);
        Notify();
    }

    public async Task ToggleVideoAsync()
    {
        IsVideoEnabled = !IsVideoEnabled;
        await _js.InvokeVoidAsync("setVideoEnabled", IsVideoEnabled);
        Notify();
    }

    public async Task ToggleScreenShareAsync()
    {
        if (IsSharingScreen)
        {
            await _js.InvokeVoidAsync("stopScreenShare");
            IsSharingScreen = false;
            IsSharingLocalScreen = false;
        }
        else
        {
            var started = await _js.InvokeAsync<bool>("startScreenShare");
            if (started)
            {
                IsSharingScreen = true;
                IsSharingLocalScreen = true;
                await _js.InvokeVoidAsync("attachLocalScreenStream");
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
    // Device management
    // -------------------------------------------------------------------------

    public async Task LoadDevicesAsync()
    {
        var result = await _js.InvokeAsync<DeviceEnumerationResult>("enumerateDevices");
        AudioDevices = result.AudioDevices.Select(d => new MediaDevice(d.DeviceId, d.Label)).ToList();
        VideoDevices = result.VideoDevices.Select(d => new MediaDevice(d.DeviceId, d.Label)).ToList();

        if (SelectedAudioDeviceId == null && AudioDevices.Count > 0)
            SelectedAudioDeviceId = AudioDevices[0].DeviceId;
        if (SelectedVideoDeviceId == null && VideoDevices.Count > 0)
            SelectedVideoDeviceId = VideoDevices[0].DeviceId;

        Notify();
    }

    public async Task SwitchAudioDeviceAsync(string deviceId)
    {
        SelectedAudioDeviceId = deviceId;
        await _js.InvokeVoidAsync("switchAudioDevice", deviceId);
        Notify();
    }

    public async Task SwitchVideoDeviceAsync(string deviceId)
    {
        SelectedVideoDeviceId = deviceId;
        await _js.InvokeVoidAsync("switchVideoDevice", deviceId);
        Notify();
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private void Notify() => OnChange?.Invoke();

    public async ValueTask DisposeAsync()
    {
        _callService.OnCallMetadataUpdated -= HandleCallMetadataUpdated;
        _callService.OnUserListUpdated -= HandleUserListUpdated;
        _callService.OnVideoOffer -= HandleVideoOffer;
        _callService.OnVideoAnswer -= HandleVideoAnswer;
        _callService.OnIceCandidate -= HandleIceCandidate;
        _callService.OnHangUp -= HandleHangUp;

        if (IsInCall)
            await _js.InvokeVoidAsync("hangUpAll", new { keepLocalStream = false });
        else if (IsPreviewing)
            await _js.InvokeVoidAsync("stopLocalPreview");

        _objRef?.Dispose();
        await _callService.DisconnectAsync();
    }

    private record DeviceEnumerationResult(List<DeviceInfo> AudioDevices, List<DeviceInfo> VideoDevices);
    private record DeviceInfo(string DeviceId, string Label);
}
