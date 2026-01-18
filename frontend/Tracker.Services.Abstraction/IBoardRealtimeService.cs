using Tracker.Domain.Events;

namespace Tracker.Services.Abstraction;

public interface IBoardRealtimeService : IAsyncDisposable
{
    Task ConnectAndJoinBoardAsync(Guid boardId);
    Task DisconnectAsync();
    bool IsConnected { get; }

    event Action<ItemMovedEvent>? OnItemMoved;
}