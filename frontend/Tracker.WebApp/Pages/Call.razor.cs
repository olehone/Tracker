using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Tracker.Services.Abstraction.Realtime;
using Tracker.Services.Abstraction.Realtime.Events;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Pages;

public record PeerState(bool Audio, bool Video, bool Screen);

public partial class Call : IAsyncDisposable
{
    private bool _connecting = false;
    private Guid _callId = Guid.Parse("29063d2a-7bfb-4384-84b7-0f8625677b0b");
    private DotNetObjectReference<Call>? _objRef;
    private List<string> _remoteUsers = new();
    private HashSet<string> _remoteScreenUsers = new();
    private Dictionary<string, PeerState> _peerStates = new();

    private bool _isMuted = false;
    private bool _isVideoEnabled = true;
    private bool _isSharingScreen = false;

    [Inject] IJSRuntime JS { get; set; } = null!;
    [Inject] ICallRealtimeService CallService { get; set; } = null!;
    [Inject] AppState AppState { get; set; } = null!;

    public bool IsUnauthenticated => AppState.IsUnauthenticated;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _objRef = DotNetObjectReference.Create(this);
            await JS.InvokeVoidAsync("registerDotNetInstance", _objRef);
        }

        foreach (var userId in _remoteUsers)
        {
            await JS.InvokeVoidAsync("attachRemoteStream", userId);
        }

        foreach (var userId in _remoteScreenUsers)
        {
            await JS.InvokeVoidAsync("attachRemoteScreenStream", userId);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        AppState.OnUserChange += OnAppStateChanged;

        if (!IsUnauthenticated)
        {
            await ConnectRealtimeAsync();
        }
    }

    private async void OnAppStateChanged()
    {
        if (!IsUnauthenticated && !_connecting)
        {
            await ConnectRealtimeAsync();
        }
    }

    private async Task ConnectRealtimeAsync()
    {
        if (_connecting)
        {
            return;
        }

        _connecting = true;

        CallService.OnUserListUpdated += HandleUserListUpdated;
        CallService.OnVideoOffer += HandleVideoOffer;
        CallService.OnVideoAnswer += HandleVideoAnswer;
        CallService.OnIceCandidate += HandleIceCandidate;
        CallService.OnHangUp += HandleHangUp;

        await CallService.ConnectAsync(_callId);
    }

    private async void HandleUserListUpdated(UserListUpdatedEvent evt)
    {
        var myId = AppState.MyId.ToString();
        var otherUsers = evt.UserIds.Where(id => id != myId).ToList();

        foreach (var userId in otherUsers)
        {
            if (!_remoteUsers.Contains(userId))
            {
                await JS.InvokeVoidAsync("initiateCall", userId, myId);
            }
        }
    }

    private void HandleVideoOffer(VideoOfferEvent evt)
    {
        JS.InvokeVoidAsync("receiveVideoOffer", evt.FromUserId, evt.Sdp);
    }

    private void HandleVideoAnswer(VideoAnswerEvent evt)
    {
        JS.InvokeVoidAsync("receiveVideoAnswer", evt.FromUserId, evt.Sdp);
    }

    private void HandleIceCandidate(IceCandidateEvent evt)
    {
        JS.InvokeVoidAsync("receiveIceCandidate", evt.FromUserId, evt.CandidateJson);
    }

    private void HandleHangUp(HangUpEvent evt)
    {
        JS.InvokeVoidAsync("receiveHangUp", evt.FromUserId);
        _remoteUsers.Remove(evt.FromUserId);
        _remoteScreenUsers.Remove(evt.FromUserId);
        _peerStates.Remove(evt.FromUserId);
        InvokeAsync(StateHasChanged);
    }

    [JSInvokable]
    public Task SendVideoOffer(string targetUserId, string sdp)
        => CallService.SendVideoOffer(_callId, targetUserId, sdp);

    [JSInvokable]
    public Task SendVideoAnswer(string targetUserId, string sdp)
        => CallService.SendVideoAnswer(_callId, targetUserId, sdp);

    [JSInvokable]
    public Task SendIceCandidate(string targetUserId, string candidateJson)
        => CallService.SendIceCandidate(_callId, targetUserId, candidateJson);

    [JSInvokable]
    public Task SendHangUp(string targetUserId)
        => CallService.SendHangUp(_callId, targetUserId);

    [JSInvokable]
    public async Task OnRemoteTrack(string userId)
    {
        if (!_remoteUsers.Contains(userId))
        {
            _remoteUsers.Add(userId);
        }

        await InvokeAsync(StateHasChanged);
    }

    [JSInvokable]
    public async Task OnRemoteScreenTrack(string userId)
    {
        _remoteScreenUsers.Add(userId);
        await InvokeAsync(StateHasChanged);
    }

    [JSInvokable]
    public async Task OnPeerDisconnected(string userId)
    {
        _remoteUsers.Remove(userId);
        _remoteScreenUsers.Remove(userId);
        _peerStates.Remove(userId);
        await InvokeAsync(StateHasChanged);
    }

    [JSInvokable]
    public async Task OnPeerStateChanged(string userId, bool audio, bool video, bool screen)
    {
        _peerStates[userId] = new PeerState(audio, video, screen);
        if (!screen)
        {
            _remoteScreenUsers.Remove(userId);
        }
        await InvokeAsync(StateHasChanged);
    }

    [JSInvokable]
    public async Task OnLocalScreenStopped()
    {
        _isSharingScreen = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task ToggleMute()
    {
        _isMuted = !_isMuted;
        await JS.InvokeVoidAsync("setMuted", _isMuted);
    }

    private async Task ToggleVideo()
    {
        _isVideoEnabled = !_isVideoEnabled;
        await JS.InvokeVoidAsync("setVideoEnabled", _isVideoEnabled);
    }

    private async Task ToggleScreenShare()
    {
        if (_isSharingScreen)
        {
            await JS.InvokeVoidAsync("stopScreenShare");
            _isSharingScreen = false;
        }
        else
        {
            var started = await JS.InvokeAsync<bool>("startScreenShare");
            if (started)
            {
                _isSharingScreen = true;
            }
        }
    }

    private async Task HangUpCall()
    {
        await JS.InvokeVoidAsync("hangUpAll");
    }

    public async ValueTask DisposeAsync()
    {
        AppState.OnUserChange -= OnAppStateChanged;
        CallService.OnUserListUpdated -= HandleUserListUpdated;
        CallService.OnVideoOffer -= HandleVideoOffer;
        CallService.OnVideoAnswer -= HandleVideoAnswer;
        CallService.OnIceCandidate -= HandleIceCandidate;
        CallService.OnHangUp -= HandleHangUp;
        await JS.InvokeVoidAsync("hangUpAll");
        _objRef?.Dispose();
        await CallService.DisconnectAsync();
    }
}
