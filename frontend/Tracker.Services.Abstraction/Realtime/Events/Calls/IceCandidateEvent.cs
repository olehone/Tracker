namespace Tracker.Services.Abstraction.Realtime.Events.Calls;

public record IceCandidateEvent(string FromUserId, string CandidateJson);