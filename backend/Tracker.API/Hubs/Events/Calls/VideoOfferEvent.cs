namespace Tracker.API.Hubs.Events.Calls;

public record VideoOfferEvent(string FromUserId, string Sdp);
