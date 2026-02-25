using Tracker.Domain.Dtos;

namespace Tracker.API.Hubs.Events;

public sealed record ListCreatedEvent(
    Guid UserId,
    Guid BoardId,
    BoardListDto List
);

public sealed record ListDeletedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ListId
);

public sealed record ListMovedEvent(
    Guid UserId,
    Guid BoardId,
    Guid ListId,
    int Position
);

public sealed record ListUpdatedEvent(
    Guid UserId,
    Guid BoardId,
    BoardListDto List
);