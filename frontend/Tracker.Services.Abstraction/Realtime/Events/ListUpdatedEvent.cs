using Tracker.Domain.Dtos;

namespace Tracker.Services.Abstraction.Realtime.Events;

public sealed record ListUpdatedEvent(
    Guid UserId,
    Guid BoardId,
    BoardListDto List
);