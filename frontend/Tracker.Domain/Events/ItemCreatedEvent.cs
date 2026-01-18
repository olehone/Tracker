using Tracker.Domain.Dtos;

namespace Tracker.Domain.Events;

public sealed record ItemCreatedEvent(
    Guid UserId,
    Guid BoardId,
    BoardItemDto Item
);