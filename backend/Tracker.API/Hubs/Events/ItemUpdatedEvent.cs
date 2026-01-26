using Tracker.API.Requests;

namespace Tracker.API.Hubs.Events;

public sealed record ItemUpdatedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ItemId,
    UpdateBoardItemRequest ChangedFields
);