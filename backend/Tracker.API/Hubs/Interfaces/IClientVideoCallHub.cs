namespace Tracker.API.Hubs.Interfaces;

public interface IClientVideoCallHub
{
    Task UserListUpdated(string[] userIds);
    Task ReceiveVideoOffer(string fromUserId, string sdp);
    Task ReceiveVideoAnswer(string fromUserId, string sdp);
    Task ReceiveIceCandidate(string fromUserId, string candidateJson);
    Task ReceiveHangUp(string fromUserId);

    /// <summary>
    /// Sent to both full participants and peekers whenever call membership changes.
    /// <paramref name="startedAt"/> is null when the call has no participants.
    /// </summary>
    Task ReceiveCallMetadata(int participantCount, DateTimeOffset? startedAt);
}
