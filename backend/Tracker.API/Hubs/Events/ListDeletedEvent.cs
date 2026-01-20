namespace Tracker.API.Hubs.Events;

public sealed record ListDeletedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ListId
);