namespace Tracker.API.Hubs.Events.Lists;

public sealed record ListMovedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ListId,
    int Position
);
