using Tracker.Services.Abstraction.Realtime.Events.Comments;

namespace Tracker.Services.Abstraction.Realtime;

public interface IItemRealtimeService : IRealtimeService
{
    event Action<CommentCreatedEvent>? OnCommentCreated;
    event Action<CommentUpdatedEvent>? OnCommentUpdated;
    event Action<CommentDeletedEvent>? OnCommentDeleted;
}