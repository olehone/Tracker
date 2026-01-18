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
        var result = await mediator.Send(new CheckBoardRealtimeAccessQuery { BoardId = boardId });
        if (result.IsFailure)
        {
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"board:{boardId}");
    }

    public async Task LeaveBoard(Guid boardId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"board:{boardId}");
    }

    public Task ItemMoved(MoveBoardItemRequest request, Guid boardId)
    {
        if (!Guid.TryParse(Context.UserIdentifier!, out Guid userId))
        {
            return Task.CompletedTask;
        }
        var evn = new ItemMovedEvent(
            BoardId: boardId,
            ToBoardListId: request.ToBoardListId,
            BoardItemId: request.BoardItemId,
            Position: request.Position,
            UserId: userId
        );
        return Clients.OthersInGroup($"board:{boardId}").ItemMoved(evn);
    }

}
