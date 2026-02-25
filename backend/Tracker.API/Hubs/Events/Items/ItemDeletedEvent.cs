namespace Tracker.API.Hubs.Events.Items;

public sealed record ItemDeletedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ItemId
);
