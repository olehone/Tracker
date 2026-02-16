namespace Tracker.API.Hubs.Events;

public sealed record CommentDeletedEvent(
    Guid UserId,
    Guid ItemId,
    Guid CommentId
);