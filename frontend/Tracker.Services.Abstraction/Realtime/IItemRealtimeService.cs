using Tracker.Services.Abstraction.Realtime.Events;
using Tracker.Services.Abstraction.Realtime.Events;

namespace Tracker.Services.Abstraction.Realtime;

public interface IItemRealtimeService : IRealtimeService
{
    event Action<CommentCreatedEvent>? OnCommentCreated;
    event Action<CommentUpdatedEvent>? OnCommentUpdated;
    event Action<CommentDeletedEvent>? OnCommentDeleted;
}