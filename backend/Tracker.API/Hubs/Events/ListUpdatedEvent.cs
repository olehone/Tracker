using Tracker.Domain.Dtos;

namespace Tracker.API.Hubs.Events;

public sealed record ListUpdatedEvent(
    Guid UserId,
    Guid BoardId,
    BoardListDto List
);