using Tracker.Domain.Requests.BoardItem;

namespace Tracker.Services.Abstraction.Realtime.Events;

public sealed record ItemUpdatedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ItemId,
    UpdateBoardItemRequest ChangedFields
);