using Tracker.Domain.Dtos;

namespace Tracker.Domain.Events;

public sealed record CommentCreatedEvent(
    Guid UserId,
    Guid ItemId,
    ItemCommentDto Comment
);