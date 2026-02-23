namespace Tracker.API.Hubs.Interfaces;

public interface IClientVideoCallHub
{
    Task UserListUpdated(string[] userIds);
    Task ReceiveVideoOffer(string fromUserId, string sdp);
    Task ReceiveVideoAnswer(string fromUserId, string sdp);
    Task ReceiveIceCandidate(string fromUserId, string candidateJson);
    Task ReceiveHangUp(string fromUserId);
    Task ReceiveCallMetadata(int participantCount, DateTimeOffset? startedAt);
}