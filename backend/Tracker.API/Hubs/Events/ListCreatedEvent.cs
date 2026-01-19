using Tracker.Domain.Dtos;

namespace Tracker.API.Hubs.Events;

public sealed record ListCreatedEvent(
    Guid UserId,
    Guid BoardId,
    BoardListDto List
);