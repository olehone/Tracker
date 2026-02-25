using Tracker.Domain.Dtos;

namespace Tracker.API.Hubs.Events;

public record CallEndedEvent();
public record UserJoinedEvent(UserDto User);
public record UserLeavedEvent(UserDto User);

public record VideoOfferEvent(string FromUserId, string Sdp);

public record VideoAnswerEvent(string FromUserId, string Sdp);

public record IceCandidateEvent(string FromUserId, string CandidateJson);

public record HangUpEvent(string FromUserId);