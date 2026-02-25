namespace Tracker.Services.Abstraction.Realtime.Events.Items;

public sealed record ItemDeletedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ItemId
);
