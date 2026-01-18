using MediatR;
using Microsoft.AspNetCore.SignalR;
using Tracker.Application.Events;

namespace Tracker.Infrastructure.SignalR;

public class ItemMovedSignalRHandler (IHubContext<BoardHub, IClientBoardHub> hub) : INotificationHandler<ItemMovedEvent>
{
    public Task Handle(ItemMovedEvent evn, CancellationToken cancellationToken)
    {
        return hub.Clients
            .Group($"board:{evn.BoardId}")
            .ItemMoved(evn);
    }
}