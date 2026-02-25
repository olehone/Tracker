namespace Tracker.Services.Abstraction.Realtime.Events.Calls;

public record VideoAnswerEvent(string FromUserId, string Sdp);
