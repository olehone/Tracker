using Tracker.Domain.Events;

namespace Tracker.Services.Abstraction.Realtime;

public interface IItemRealtimeService : IAsyncDisposable
{
    Task ConnectAsync(Guid itemId);
    Task DisconnectAsync();
    bool IsConnected { get; }

    event Action<CommentCreatedEvent>? OnCommentCreated;
    event Action<CommentUpdatedEvent>? OnCommentUpdated;
    event Action<CommentDeletedEvent>? OnCommentDeleted;
}