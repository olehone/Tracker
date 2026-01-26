using Tracker.Domain.Requests.BoardItem;

namespace Tracker.Domain.Events;

public sealed record ItemUpdatedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ItemId,
    UpdateBoardItemRequest ChangedFields
);