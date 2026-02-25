namespace Tracker.API.Hubs.Events.Calls;

public record VideoAnswerEvent(string FromUserId, string Sdp);
