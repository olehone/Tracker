using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Tracker.API.Hubs;

[Authorize]
public class VideoCallHub() : Hub<IClientVideoCallHub>
{
    public async Task JoinCall(Guid callId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"call:{callId}");
    }

    public async Task LeaveCall(Guid callId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"call:{callId}");
    }

}
