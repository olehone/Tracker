namespace Tracker.Services.Abstraction.Realtime.Events;

public sealed record ListMovedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ListId,
    int Position
);