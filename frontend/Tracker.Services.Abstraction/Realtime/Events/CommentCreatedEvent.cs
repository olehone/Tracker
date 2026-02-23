using Tracker.Domain.Dtos;

namespace Tracker.Services.Abstraction.Realtime.Events;

public sealed record CommentCreatedEvent(
    Guid UserId,
    Guid ItemId,
    ItemCommentDto Comment
);