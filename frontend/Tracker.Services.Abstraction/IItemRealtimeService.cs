using Tracker.Domain.Events;

namespace Tracker.Services.Abstraction;

public interface IItemRealtimeService : IAsyncDisposable
{
    Task ConnectAndJoinItemAsync(Guid itemId);
    Task DisconnectAsync();
    bool IsConnected { get; }

    event Action<CommentCreatedEvent>? OnCommentCreated;
    event Action<CommentUpdatedEvent>? OnCommentUpdated;
    event Action<CommentDeletedEvent>? OnCommentDeleted;
}