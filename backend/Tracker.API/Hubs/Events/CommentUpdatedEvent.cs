using Tracker.Domain.Dtos;

namespace Tracker.API.Hubs.Events;

public sealed record CommentUpdatedEvent(
    Guid UserId,
    Guid ItemId,
    ItemCommentDto Comment
);