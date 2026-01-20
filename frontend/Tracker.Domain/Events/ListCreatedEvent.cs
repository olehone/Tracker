using Tracker.Domain.Dtos;

namespace Tracker.Domain.Events;

public sealed record ListCreatedEvent(
    Guid UserId,
    Guid BoardId,
    BoardListDto List
);