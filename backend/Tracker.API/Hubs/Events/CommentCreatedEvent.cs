using Tracker.Domain.Dtos;

namespace Tracker.API.Hubs.Events;

public sealed record CommentCreatedEvent(
    Guid UserId,
    Guid ItemId,
    ItemCommentDto Comment
);