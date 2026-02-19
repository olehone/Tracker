using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Tracker.API.Hubs.Interfaces;
using Tracker.Application.UseCases.BoardItems.CheckRealtimeAccess;

namespace Tracker.API.Hubs;

[Authorize]
public class ItemHub(IMediator mediator) : Hub<IClientItemHub>
{
    public async Task Join(Guid itemId)
    {
        var result = await mediator.Send(new CheckItemRealtimeAccessQuery
        {
            ItemId = itemId,
        });

        if (result.IsFailure)
        {
            throw new HubException(result.Error.Description);
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"item:{itemId}");
    }

    public async Task Leave(Guid itemId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"item:{itemId}");
    }

}