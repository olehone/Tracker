using Tracker.Domain.Dtos;

namespace Tracker.Services.Abstraction.Realtime.Events;

public record UserJoinedEvent(UserDto User);
public record UserLeavedEvent(string UserId);

public record VideoOfferEvent(string FromUserId, string Sdp);

public record VideoAnswerEvent(string FromUserId, string Sdp);

public record IceCandidateEvent(string FromUserId, string CandidateJson);