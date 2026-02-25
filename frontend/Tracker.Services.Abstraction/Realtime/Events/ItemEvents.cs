using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;

namespace Tracker.Services.Abstraction.Realtime.Events;

public sealed record ItemCreatedEvent(
    Guid UserId,
    Guid BoardId,
    BoardItemDto Item
);

public sealed record ItemDeletedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ItemId
);

public sealed record ItemMovedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ToListId,
    Guid ItemId,
    int Position
);

public sealed record ItemUpdatedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ItemId,
    UpdateBoardItemRequest ChangedFields
);