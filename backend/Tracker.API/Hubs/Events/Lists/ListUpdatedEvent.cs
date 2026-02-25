using Tracker.Domain.Dtos;

namespace Tracker.API.Hubs.Events.Lists;

public sealed record ListUpdatedEvent(
    Guid UserId,
    Guid BoardId,
    BoardListDto List
);