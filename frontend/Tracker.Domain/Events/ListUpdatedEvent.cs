using Tracker.Domain.Dtos;

namespace Tracker.Domain.Events;

public sealed record ListUpdatedEvent(
    Guid UserId,
    Guid BoardId,
    BoardListDto List
);