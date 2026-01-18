using Tracker.Domain.Dtos;

namespace Tracker.API.Hubs.Events;

public sealed record ItemCreatedEvent(
    Guid UserId,
    Guid BoardId,
    BoardItemDto Item
);