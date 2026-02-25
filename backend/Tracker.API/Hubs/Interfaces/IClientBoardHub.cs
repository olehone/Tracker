using Tracker.API.Hubs.Events.Calls;
using Tracker.API.Hubs.Events.Items;
using Tracker.API.Hubs.Events.Lists;

namespace Tracker.API.Hubs.Interfaces;

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

    Task CallStarted(BoardCallStartedEvent evt);
}