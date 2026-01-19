namespace Tracker.Domain.Events;

public sealed record ItemDeletedEvent(
    Guid UserId,
    Guid BoardId,
    Guid BoardItemId
);