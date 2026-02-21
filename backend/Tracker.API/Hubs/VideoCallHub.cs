using System.Collections.Concurrent;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Tracker.API.Hubs.Events;
using Tracker.API.Hubs.Interfaces;
using Tracker.Application.UseCases.Users.Current;
using Tracker.Infrastructure.Auth;

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
        var userId = user.Value.Id.ToString();
        _users[userId] = Context.ConnectionId;

        await Groups.AddToGroupAsync(Context.ConnectionId, $"call:{callId}");

        await BroadcastUserList(callId);
    }

    public async Task Leave(Guid callId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"call:{callId}");
        RemoveCurrentUser();
        await BroadcastUserList(callId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        RemoveCurrentUser();
        await base.OnDisconnectedAsync(exception);
    }

    private void RemoveCurrentUser()
    {
        var entry = _users.FirstOrDefault(x => x.Value == Context.ConnectionId);
        if (entry.Key != null)
        {
            _users.TryRemove(entry.Key, out _);
        }
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

    public async Task SendData(Guid callId, string data)
    {
        Console.WriteLine($"Sending data to {callId}. Data is {data}");
        await Clients.OthersInGroup($"call:{callId}").DataSent(data);
    }

    public async Task SendVideoOffer(VideoOfferEvent evt)
    {
        Console.WriteLine($"Sending data to {evt.CallerId}. Data is {evt.SessionDescriptionProtocol}");
        await Clients.OthersInGroup($"call:{callId}").DataSent(data);
    }
}