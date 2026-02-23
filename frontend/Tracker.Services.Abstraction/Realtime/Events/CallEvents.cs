namespace Tracker.Services.Abstraction.Realtime.Events;

public record UserListUpdatedEvent(string[] UserIds);

public record VideoOfferEvent(string FromUserId, string Sdp);

public record VideoAnswerEvent(string FromUserId, string Sdp);

public record IceCandidateEvent(string FromUserId, string CandidateJson);

public record HangUpEvent(string FromUserId);

public record CallMetadataEvent(int ParticipantCount, DateTimeOffset? StartedAt);
