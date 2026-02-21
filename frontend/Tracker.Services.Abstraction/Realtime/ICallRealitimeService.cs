using Tracker.Services.Abstraction.Realtime.Events;

namespace Tracker.Services.Abstraction.Realtime;

public interface ICallRealtimeService : IRealtimeService
{
    event Action<UserListUpdatedEvent>? OnUserListUpdated;
    event Action<VideoOfferEvent>? OnVideoOffer;
    event Action<VideoAnswerEvent>? OnVideoAnswer;
    event Action<IceCandidateEvent>? OnIceCandidate;
    event Action<HangUpEvent>? OnHangUp;

    Task SendVideoOffer(Guid callId, string targetUserId, string sdp);
    Task SendVideoAnswer(Guid callId, string targetUserId, string sdp);
    Task SendIceCandidate(Guid callId, string targetUserId, string candidateJson);
    Task SendHangUp(Guid callId, string targetUserId);
}
