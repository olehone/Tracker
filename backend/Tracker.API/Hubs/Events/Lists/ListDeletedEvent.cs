namespace Tracker.API.Hubs.Events.Lists;

public sealed record ListDeletedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ListId
);
