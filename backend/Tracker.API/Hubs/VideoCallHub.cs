using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Tracker.API.Hubs;

//[Authorize]
public class VideoCallHub() : Hub<IClientVideoCallHub>
{
    private static readonly ConcurrentDictionary<string, string> _users = new();

    public async Task JoinCall(Guid callId)
    {
        var username = Context.User?.Identity?.Name ?? Context.ConnectionId[..8];
        _users[Context.ConnectionId] = username;

        await Groups.AddToGroupAsync(Context.ConnectionId, $"call:{callId}");

        // Broadcast updated user list to the group
        await BroadcastUserList(callId);

        // Announce the new user
        var joinMsg = JsonSerializer.Serialize(new
        {
            type = "username",
            name = username,
            date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
        await Clients.Group($"call:{callId}").SendData(joinMsg);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _users.TryRemove(Context.ConnectionId, out _);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendData(Guid callId, string data)
    {
        await Clients.Group($"call:{callId}").SendData(data);
    }

    private async Task BroadcastUserList(Guid callId)
    {
        var userListMsg = JsonSerializer.Serialize(new
        {
            type = "userlist",
            users = _users.Values.ToArray(),
            date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
        await Clients.Group($"call:{callId}").SendData(userListMsg);
    }
}
