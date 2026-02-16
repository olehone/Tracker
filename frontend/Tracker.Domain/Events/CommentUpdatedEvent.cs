using Tracker.Domain.Dtos;

namespace Tracker.Domain.Events;

public sealed record CommentUpdatedEvent(
    Guid UserId,
    Guid ItemId,
    ItemCommentDto Comment
);