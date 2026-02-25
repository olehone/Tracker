using Tracker.Domain.Dtos;

namespace Tracker.Services.Abstraction.Realtime.Events.Lists;

public sealed record ListCreatedEvent(
    Guid UserId,
    Guid BoardId,
    BoardListDto List
);
