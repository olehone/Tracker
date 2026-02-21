using System.Collections.Concurrent;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Tracker.API.Hubs.Events;
using Tracker.API.Hubs.Interfaces;
using Tracker.Application.UseCases.Users.Current;

namespace Tracker.API.Hubs;

[Authorize]
public class VideoCallHub(IMediator mediator) : Hub<IClientVideoCallHub>
{
    private static readonly ConcurrentDictionary<string, string> _userConnections = new(); // userId -> connectionId
    private static readonly ConcurrentDictionary<string, string> _userCalls = new();       // userId -> callId

    public async Task Join(Guid callId)
    {
        var user = await mediator.Send(new GetCurrentUserQuery());
        if (user.IsFailure)
            throw new HubException("Unauthorized");

        var userId = user.Value.Id.ToString();
        var callIdStr = callId.ToString();

        _userConnections[userId] = Context.ConnectionId;
        _userCalls[userId] = callIdStr;

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
        var entry = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);
        if (entry.Key == null)
            return;
        _userConnections.TryRemove(entry.Key, out _);
        _userCalls.TryRemove(entry.Key, out _);
    }

    private async Task BroadcastUserList(Guid callId)
    {
        var callIdStr = callId.ToString();
        var usersInCall = _userCalls
            .Where(kvp => kvp.Value == callIdStr)
            .Select(kvp => kvp.Key)
            .ToArray();

        var msg = JsonSerializer.Serialize(new
        {
            type = "userlist",
            users = usersInCall,
            date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });

        await Clients.Group($"call:{callId}").DataSent(msg);
    }

    public async Task SendData(Guid callId, string data)
    {
        using var doc = JsonDocument.Parse(data);
        if (!doc.RootElement.TryGetProperty("target", out var targetEl))
            return;
        var targetUserId = targetEl.GetString();
        if (targetUserId == null)
            return;

        if (!_userCalls.TryGetValue(targetUserId, out var targetCallId))
            return;
        if (targetCallId != callId.ToString())
            return;
        if (!_userConnections.TryGetValue(targetUserId, out var connectionId))
            return;

        var senderId = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;

        // inject name into payload
        var dict = new Dictionary<string, object>();
        foreach (var prop in doc.RootElement.EnumerateObject())
            dict[prop.Name] = prop.Value;
        dict["name"] = senderId;

        var enriched = JsonSerializer.Serialize(dict);
        await Clients.Client(connectionId).DataSent(enriched);
    }
}