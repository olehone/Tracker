namespace Tracker.Domain.Events;

public sealed record ItemMovedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ToBoardListId,
    Guid BoardItemId,
    int Position
);