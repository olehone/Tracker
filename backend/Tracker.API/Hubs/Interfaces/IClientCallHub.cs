using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;

namespace Tracker.API.Hubs.Interfaces;

public interface IClientCallHub
{
    Task ReceiveVideoOffer(string fromUserId, string sdp);
    Task ReceiveVideoAnswer(string fromUserId, string sdp);
    Task ReceiveIceCandidate(string fromUserId, string candidateJson);
    Task ReceiveHangUp(string fromUserId);

    Task CallUpdated(CallDto call);
    Task CallEnded();

    Task UserJoined(UserDto user);
    Task UserLeaved(Guid userId);
}