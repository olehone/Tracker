using Tracker.Domain.Events;

namespace Tracker.Services.Abstraction.Realtime;

public interface IItemRealtimeService : IRealtimeService
{
    event Action<CommentCreatedEvent>? OnCommentCreated;
    event Action<CommentUpdatedEvent>? OnCommentUpdated;
    event Action<CommentDeletedEvent>? OnCommentDeleted;
}