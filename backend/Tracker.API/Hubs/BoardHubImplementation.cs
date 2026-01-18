using Microsoft.AspNetCore.Authorization;
using Tracker.Infrastructure.SignalR;

namespace Tracker.API.Hubs;

public class BoardHubImplementation: BoardHub
{
    public Task JoinBoard(Guid boardId)
    {
        return Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"board:{boardId}");
    }

    public async Task LeaveBoard(Guid boardId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"board-{boardId}");
    }
}
