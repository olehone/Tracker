using Tracker.Domain.Dtos;

namespace Tracker.Services.Abstraction.Realtime.Events;

public sealed record ListCreatedEvent(
    Guid UserId,
    Guid BoardId,
    BoardListDto List
);