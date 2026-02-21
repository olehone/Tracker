namespace Tracker.Services.Abstraction.Realtime.Events;

public sealed record VideoOfferEvent(Guid CallerId, string SessionDescriptionProtocol);