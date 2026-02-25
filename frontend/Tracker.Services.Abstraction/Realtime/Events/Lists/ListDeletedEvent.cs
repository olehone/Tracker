namespace Tracker.Services.Abstraction.Realtime.Events.Lists;

public sealed record ListDeletedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ListId
);
