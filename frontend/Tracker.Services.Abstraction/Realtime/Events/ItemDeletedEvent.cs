namespace Tracker.Services.Abstraction.Realtime.Events;

public sealed record ItemDeletedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ItemId
);