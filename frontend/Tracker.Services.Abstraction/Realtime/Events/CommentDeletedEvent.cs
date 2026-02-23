namespace Tracker.Services.Abstraction.Realtime.Events;

public sealed record CommentDeletedEvent(
    Guid UserId,
    Guid ItemId,
    Guid CommentId
);