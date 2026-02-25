namespace Tracker.Services.Abstraction.Realtime.Events.Calls;

public record VideoOfferEvent(string FromUserId, string Sdp);
