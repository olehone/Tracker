namespace Tracker.Services.Abstraction.Realtime.Events.Comments;

public sealed record CommentDeletedEvent(
    Guid UserId,
    Guid ItemId,
    Guid CommentId
);