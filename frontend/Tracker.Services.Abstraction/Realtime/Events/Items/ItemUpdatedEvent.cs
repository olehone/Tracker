using Tracker.Domain.Requests.BoardItem;

namespace Tracker.Services.Abstraction.Realtime.Events.Items;

public sealed record ItemUpdatedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ItemId,
    UpdateBoardItemRequest ChangedFields
);