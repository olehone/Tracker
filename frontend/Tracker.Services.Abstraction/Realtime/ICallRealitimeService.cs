using Tracker.Services.Abstraction.Realtime.Events;

namespace Tracker.Services.Abstraction.Realtime;

public interface ICallRealtimeService : IAsyncDisposable
{
    event Action<UserListUpdatedEvent>? OnUserListUpdated;
    event Action<VideoOfferEvent>? OnVideoOffer;
    event Action<VideoAnswerEvent>? OnVideoAnswer;
    event Action<IceCandidateEvent>? OnIceCandidate;
    event Action<HangUpEvent>? OnHangUp;
    event Action<CallMetadataEvent>? OnCallMetadataUpdated;

    Task ConnectAsync(Guid callId);
    Task DisconnectAsync();

    Task PeekAsync(Guid callId);

    Task SendVideoOffer(Guid callId, string targetUserId, string sdp);
    Task SendVideoAnswer(Guid callId, string targetUserId, string sdp);
    Task SendIceCandidate(Guid callId, string targetUserId, string candidateJson);
    Task SendHangUp(Guid callId, string targetUserId);
}
