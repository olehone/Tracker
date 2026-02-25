using Tracker.Domain.Dtos;

namespace Tracker.API.Hubs.Events.Items;

public sealed record ItemCreatedEvent(
    Guid UserId,
    Guid BoardId,
    BoardItemDto Item
);
