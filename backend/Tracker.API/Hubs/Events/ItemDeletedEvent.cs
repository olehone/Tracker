namespace Tracker.API.Hubs.Events;

public sealed record ItemDeletedEvent(
    Guid UserId,
    Guid BoardId,
    Guid BoardItemId
);