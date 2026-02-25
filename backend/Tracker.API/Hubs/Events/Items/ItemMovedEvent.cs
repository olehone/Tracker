namespace Tracker.API.Hubs.Events.Items;

public sealed record ItemMovedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ToListId,
    Guid ItemId,
    int Position
);
