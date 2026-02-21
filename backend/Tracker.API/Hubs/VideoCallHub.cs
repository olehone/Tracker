using System.Collections.Concurrent;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Tracker.API.Hubs.Interfaces;
using Tracker.Application.UseCases.Users.Current;

namespace Tracker.API.Hubs;

[Authorize]
public class VideoCallHub(IMediator mediator) : Hub<IClientVideoCallHub>
{
    private static readonly ConcurrentDictionary<string, string> _userConnections = new();
    private static readonly ConcurrentDictionary<string, string> _userCalls = new();

    public async Task Join(Guid callId)
    {
        var userResult = await mediator.Send(new GetCurrentUserQuery());
        if (userResult.IsFailure)
        {
            throw new HubException(userResult.Error.Description);
        }

        var userId = userResult.Value.Id.ToString();

        if (_userConnections.TryGetValue(userId, out var oldConnectionId))
        {
            _userConnections.TryRemove(userId, out _);
            _userCalls.TryRemove(userId, out _);
            await Clients.OthersInGroup($"call:{callId}").ReceiveHangUp(userId);
            await Groups.RemoveFromGroupAsync(oldConnectionId, $"call:{callId}");
        }

        _userConnections[userId] = Context.ConnectionId;
        _userCalls[userId] = callId.ToString();

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

    public async Task SendVideoOffer(Guid callId, string targetUserId, string sdp)
    {
        var senderId = GetSenderId();
        if (senderId == null)
        {
            return;
        }

        var connection = GetUserConnection(callId, targetUserId);
        await Clients.Client(connection).ReceiveVideoOffer(senderId, sdp);
    }

    public async Task SendVideoAnswer(Guid callId, string targetUserId, string sdp)
    {
        var senderId = GetSenderId();
        if (senderId == null)
        {
            return;
        }

        var connection = GetUserConnection(callId, targetUserId);
        await Clients.Client(connection).ReceiveVideoAnswer(senderId, sdp);
    }

    public async Task SendIceCandidate(Guid callId, string targetUserId, string candidateJson)
    {
        var senderId = GetSenderId();
        if (senderId == null)
        {
            return;
        }

        var connection = GetUserConnection(callId, targetUserId);
        await Clients.Client(connection).ReceiveIceCandidate(senderId, candidateJson);
    }

    public async Task SendHangUp(Guid callId, string targetUserId)
    {
        var senderId = GetSenderId();
        if (senderId == null)
        {
            return;
        }
        var connection = GetUserConnection(callId, targetUserId);
        await Clients.Client(connection).ReceiveHangUp(senderId);
    }

    private string GetUserConnection(Guid callId, string targetUserId)
    {
        if (!_userCalls.TryGetValue(targetUserId, out var targetCallId))
        {
            throw new HubException("User is not on call");
        }

        if (targetCallId != callId.ToString())
        {
            throw new HubException("User is on another call");
        }

        if (!_userConnections.TryGetValue(targetUserId, out var connectionId))
        {
            throw new HubException("User connection is not found");
        }

        return connectionId;
    }

    private string? GetSenderId()
    {
        return _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
    }

    private void RemoveCurrentUser()
    {
        var entry = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);
        if (entry.Key == null)
        {
            return;
        }

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

        await Clients.Group($"call:{callId}").UserListUpdated(usersInCall);
    }
}