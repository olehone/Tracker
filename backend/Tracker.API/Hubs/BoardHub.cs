using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Tracker.API.Hubs.Events;
using Tracker.API.Requests;
using Tracker.Application.UseCases.Boards.CheckRealtimeAccess;

namespace Tracker.API.Hubs;

[Authorize]
public class BoardHub(IMediator mediator) : Hub<IClientBoardHub>
{
    public async Task JoinBoard(Guid boardId)
    {
        if (!Guid.TryParse(Context.UserIdentifier, out var userId))
        {
            throw new HubException("User not authenticated");
        }

        var result = await mediator.Send(new CheckBoardRealtimeAccessQuery
        {
            BoardId = boardId,
            UserId = userId
        });

        if (result.IsFailure)
        {
            throw new HubException(result.Error.Description);
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"board:{boardId}");
    }

    public async Task LeaveBoard(Guid boardId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"board:{boardId}");
    }

}
