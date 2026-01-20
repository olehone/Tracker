namespace Tracker.Domain.Events;

public sealed record ListDeletedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ListId
);