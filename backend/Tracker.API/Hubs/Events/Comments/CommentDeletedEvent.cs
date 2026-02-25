namespace Tracker.API.Hubs.Events.Comments;

public sealed record CommentDeletedEvent(
    Guid UserId,
    Guid ItemId,
    Guid CommentId
);