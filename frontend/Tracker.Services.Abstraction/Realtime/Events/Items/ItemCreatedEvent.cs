using Tracker.Domain.Dtos;

namespace Tracker.Services.Abstraction.Realtime.Events.Items;

public sealed record ItemCreatedEvent(
    Guid UserId,
    Guid BoardId,
    BoardItemDto Item
);
