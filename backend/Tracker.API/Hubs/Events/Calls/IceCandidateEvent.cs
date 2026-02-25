namespace Tracker.API.Hubs.Events.Calls;

public record IceCandidateEvent(string FromUserId, string CandidateJson);