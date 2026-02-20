using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Azure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Tracker.API.Hubs.Interfaces;
using Tracker.Application.UseCases.Users.Current;

namespace Tracker.API.Hubs;

[Authorize]
public class VideoCallHub(IMediator mediator) : Hub<IClientVideoCallHub>
{
    private static readonly ConcurrentDictionary<string, string> _users = new();

    public async Task Join(Guid callId)
    {
        var user = await mediator.Send(new GetCurrentUserQuery());
        if (user.IsFailure)
        {
            Console.WriteLine("Can't load user");
        }
        var username = user.Value.Id.ToString();
        _users[Context.ConnectionId] = username;

        await Groups.AddToGroupAsync(Context.ConnectionId, $"call:{callId}");

        await BroadcastUserList(callId);
    }

    public async Task Leave(Guid boardId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"board:{boardId}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _users.TryRemove(Context.ConnectionId, out _);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendData(Guid callId, string data)
    {
        Console.WriteLine($"Sending data to {callId}. Data is {data}");
        await Clients.OthersInGroup($"call:{callId}").DataSent(data);
    }

    private async Task BroadcastUserList(Guid callId)
    {
        var userListMsg = JsonSerializer.Serialize(new
        {
            type = "userlist",
            users = _users.Values.ToArray(),
            date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
        await Clients.Group($"call:{callId}").DataSent(userListMsg);
    }
}
