namespace Tracker.API.Hubs.Events;

public record UserListUpdatedEvent(string[] UserIds);

public record VideoOfferEvent(string FromUserId, string Sdp);

public record VideoAnswerEvent(string FromUserId, string Sdp);

public record IceCandidateEvent(string FromUserId, string CandidateJson);

public record HangUpEvent(string FromUserId);