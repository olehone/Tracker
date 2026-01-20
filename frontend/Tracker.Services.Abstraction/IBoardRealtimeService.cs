using Tracker.Domain.Events;

namespace Tracker.Services.Abstraction;

public interface IBoardRealtimeService : IAsyncDisposable
{
    Task ConnectAndJoinBoardAsync(Guid boardId);
    Task DisconnectAsync();
    bool IsConnected { get; }

    event Action<ItemCreatedEvent>? OnItemCreated;
    event Action<ItemMovedEvent>? OnItemMoved;
    event Action<ItemUpdatedEvent>? OnItemUpdated;
    event Action<ItemDeletedEvent>? OnItemDeleted;

    event Action<ListCreatedEvent>? OnListCreated;
    event Action<ListMovedEvent>? OnListMoved;
    event Action<ListUpdatedEvent>? OnListUpdated;
    event Action<ListDeletedEvent>? OnListDeleted;
}