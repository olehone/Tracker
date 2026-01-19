using Tracker.Domain.Dtos;

namespace Tracker.API.Hubs.Events;

public sealed record ItemUpdatedEvent(
    Guid UserId,
    Guid BoardId,
    BoardItemDto Item
);