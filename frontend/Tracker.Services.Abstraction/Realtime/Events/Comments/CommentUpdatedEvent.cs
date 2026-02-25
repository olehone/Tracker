using Tracker.Domain.Dtos;

namespace Tracker.Services.Abstraction.Realtime.Events.Comments;

public sealed record CommentUpdatedEvent(
    Guid UserId,
    Guid ItemId,
    ItemCommentDto Comment
);
