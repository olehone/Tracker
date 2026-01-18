using Tracker.Application.Events;

namespace Tracker.Infrastructure.SignalR;

public interface IClientBoardHub
{
    Task ItemMoved(ItemMovedEvent evt);
}