using Tracker.API.Hubs.Events;

namespace Tracker.API.Hubs;

public interface IClientBoardHub
{
    Task ItemMoved(ItemMovedEvent evt);
}