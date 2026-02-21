using Microsoft.AspNetCore.SignalR.Client;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Realtime;
using Tracker.Services.Abstraction.Realtime.Events;
using Tracker.Services.Realtime.Methods;

namespace Tracker.Services.Realtime;

public class CallRealtimeService(IApiUrlService apiUrl, IAuthService authService)
    : RealtimeService(apiUrl, authService, "hubs/call"), ICallRealtimeService
{
    public event Action<UserListUpdatedEvent>? OnUserListUpdated;
    public event Action<VideoOfferEvent>? OnVideoOffer;
    public event Action<VideoAnswerEvent>? OnVideoAnswer;
    public event Action<IceCandidateEvent>? OnIceCandidate;
    public event Action<HangUpEvent>? OnHangUp;

    public override void RegisterEvents(HubConnection connection)
    {
        connection.On<string[]>(CallRealtimeMethods.UserListUpdated, userIds =>
            OnUserListUpdated?.Invoke(new UserListUpdatedEvent(userIds)));

        connection.On<string, string>(CallRealtimeMethods.ReceiveVideoOffer, (fromUserId, sdp) =>
            OnVideoOffer?.Invoke(new VideoOfferEvent(fromUserId, sdp)));

        connection.On<string, string>(CallRealtimeMethods.ReceiveVideoAnswer, (fromUserId, sdp) =>
            OnVideoAnswer?.Invoke(new VideoAnswerEvent(fromUserId, sdp)));

        connection.On<string, string>(CallRealtimeMethods.ReceiveIceCandidate, (fromUserId, candidateJson) =>
            OnIceCandidate?.Invoke(new IceCandidateEvent(fromUserId, candidateJson)));

        connection.On<string>(CallRealtimeMethods.ReceiveHangUp, fromUserId =>
            OnHangUp?.Invoke(new HangUpEvent(fromUserId)));
    }

    public Task SendVideoOffer(Guid callId, string targetUserId, string sdp)
    {
        if (!IsConnected)
            return Task.CompletedTask;
        return Connection.InvokeAsync(CallRealtimeMethods.SendVideoOffer, callId, targetUserId, sdp);
    }

    public Task SendVideoAnswer(Guid callId, string targetUserId, string sdp)
    {
        if (!IsConnected)
            return Task.CompletedTask;
        return Connection.InvokeAsync(CallRealtimeMethods.SendVideoAnswer, callId, targetUserId, sdp);
    }

    public Task SendIceCandidate(Guid callId, string targetUserId, string candidateJson)
    {
        if (!IsConnected)
            return Task.CompletedTask;
        return Connection.InvokeAsync(CallRealtimeMethods.SendIceCandidate, callId, targetUserId, candidateJson);
    }

    public Task SendHangUp(Guid callId, string targetUserId)
    {
        if (!IsConnected)
            return Task.CompletedTask;
        return Connection.InvokeAsync(CallRealtimeMethods.SendHangUp, callId, targetUserId);
    }

    public override ValueTask DisposeAsync()
    {
        OnUserListUpdated = null;
        OnVideoOffer = null;
        OnVideoAnswer = null;
        OnIceCandidate = null;
        OnHangUp = null;
        return base.DisposeAsync();
    }
}
