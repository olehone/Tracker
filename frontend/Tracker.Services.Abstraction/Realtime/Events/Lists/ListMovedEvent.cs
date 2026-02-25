namespace Tracker.Services.Abstraction.Realtime.Events.Lists;

public sealed record ListMovedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ListId,
    int Position
);
