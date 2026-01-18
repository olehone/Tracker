using Tracker.API.Hubs.Events;

namespace Tracker.API.Hubs;

public interface IClientBoardHub
{
    Task ItemCreated(ItemCreatedEvent evt);
    Task ItemMoved(ItemMovedEvent evt);
    Task ItemUpdated(ItemUpdatedEvent evt);
    Task ItemDeleted(ItemDeletedEvent evt);
}