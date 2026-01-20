using Tracker.API.Hubs.Events;

namespace Tracker.API.Hubs;

public interface IClientBoardHub
{
    Task ItemCreated(ItemCreatedEvent evt);
    Task ItemMoved(ItemMovedEvent evt);
    Task ItemUpdated(ItemUpdatedEvent evt);
    Task ItemDeleted(ItemDeletedEvent evt);

    Task ListCreated(ListCreatedEvent evt);
    Task ListMoved(ListMovedEvent evt);
    Task ListUpdated(ListUpdatedEvent evt);
    Task ListDeleted(ListDeletedEvent evt);
}