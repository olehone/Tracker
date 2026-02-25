using Microsoft.AspNetCore.SignalR.Client;
using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Realtime;
using Tracker.Services.Abstraction.Realtime.Events;
using Tracker.Services.Realtime.Methods;

namespace Tracker.Services.Realtime;

public class CallRealtimeService(IApiUrlService apiUrl, IAuthService authService)
    : RealtimeService(apiUrl, authService, "hubs/call"), ICallRealtimeService
{
    public event Action? OnCallEnded;
    public event Action<UserJoinedEvent>? OnUserJoined;
    public event Action<UserLeavedEvent>? OnUserLeaved;

    public event Action<VideoOfferEvent>? OnVideoOffer;
    public event Action<VideoAnswerEvent>? OnVideoAnswer;
    public event Action<IceCandidateEvent>? OnIceCandidate;

    public override void RegisterEvents(HubConnection connection)
    {
        connection.On(CallRealtimeMethods.CallEnded, () =>
            OnCallEnded?.Invoke());

        connection.On<UserDto>(CallRealtimeMethods.UserJoined, user =>
            OnUserJoined?.Invoke(new UserJoinedEvent(user)));

        connection.On<string>(CallRealtimeMethods.UserLeaved, user =>
            OnUserLeaved?.Invoke(new UserLeavedEvent(user)));

        connection.On<string, string>(CallRealtimeMethods.ReceiveVideoOffer, (fromUserId, sdp) =>
            OnVideoOffer?.Invoke(new VideoOfferEvent(fromUserId, sdp)));

        connection.On<string, string>(CallRealtimeMethods.ReceiveVideoAnswer, (fromUserId, sdp) =>
            OnVideoAnswer?.Invoke(new VideoAnswerEvent(fromUserId, sdp)));

        connection.On<string, string>(CallRealtimeMethods.ReceiveIceCandidate, (fromUserId, candidateJson) =>
            OnIceCandidate?.Invoke(new IceCandidateEvent(fromUserId, candidateJson)));
    }

    public async Task PeekAsync(Guid callId)
    {
        await StartConnectionAsync();
        await Connection.InvokeAsync(CallRealtimeMethods.Peek, callId);
    }

    public async Task LeaveAsync(Guid callId)
    {
        await StartConnectionAsync();
        await Connection.InvokeAsync(CallRealtimeMethods.Leave, callId);
    }

    public Task SendVideoOffer(Guid callId, string targetUserId, string sdp)
    {
        if (!IsConnected)
        {
            return Task.CompletedTask;
        }

        return Connection.InvokeAsync(CallRealtimeMethods.SendVideoOffer, callId, targetUserId, sdp);
    }

    public Task SendVideoAnswer(Guid callId, string targetUserId, string sdp)
    {
        if (!IsConnected)
        {
            return Task.CompletedTask;
        }

        return Connection.InvokeAsync(CallRealtimeMethods.SendVideoAnswer, callId, targetUserId, sdp);
    }

    public Task SendIceCandidate(Guid callId, string targetUserId, string candidateJson)
    {
        if (!IsConnected)
        {
            return Task.CompletedTask;
        }

        return Connection.InvokeAsync(CallRealtimeMethods.SendIceCandidate, callId, targetUserId, candidateJson);
    }

    public Task SendHangUp(Guid callId, string targetUserId)
    {
        if (!IsConnected)
        {
            return Task.CompletedTask;
        }

        return Connection.InvokeAsync(CallRealtimeMethods.SendHangUp, callId, targetUserId);
    }

    public override ValueTask DisposeAsync()
    {
        OnCallEnded = null;
        OnUserJoined = null;
        OnUserLeaved = null;

        OnVideoOffer = null;
        OnVideoAnswer = null;
        OnIceCandidate = null;
        return base.DisposeAsync();
    }
}
