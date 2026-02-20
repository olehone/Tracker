namespace Tracker.Services.Abstraction.Realtime.Events;

public sealed record ListDeletedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ListId
);