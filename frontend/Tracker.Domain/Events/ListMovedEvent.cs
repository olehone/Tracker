namespace Tracker.Domain.Events;

public sealed record ListMovedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ListId,
    int Position
);