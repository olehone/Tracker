namespace Tracker.API.Hubs.Events;

public sealed record VideoOfferEvent(
    Guid CallerId,
    string SessionDescriptionProtocol
);