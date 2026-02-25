using Tracker.Services.Abstraction.Realtime.Events;

namespace Tracker.Services.Abstraction.Realtime;

public interface ICallRealtimeService : IAsyncDisposable
{
    event Action? OnCallEnded;
    event Action<UserJoinedEvent>? OnUserJoined;
    event Action<UserLeavedEvent>? OnUserLeaved;

    event Action<VideoOfferEvent>? OnVideoOffer;
    event Action<VideoAnswerEvent>? OnVideoAnswer;
    event Action<IceCandidateEvent>? OnIceCandidate;

    Task ConnectAsync(Guid callId);
    Task DisconnectAsync();

    Task PeekAsync(Guid callId);
    Task LeaveAsync(Guid callId);

    Task SendVideoOffer(Guid callId, string targetUserId, string sdp);
    Task SendVideoAnswer(Guid callId, string targetUserId, string sdp);
    Task SendIceCandidate(Guid callId, string targetUserId, string candidateJson);
}
