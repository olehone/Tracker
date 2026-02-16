namespace Tracker.Domain.Events;

public sealed record CommentDeletedEvent(
    Guid UserId,
    Guid ItemId,
    Guid CommentId
);