using System.Collections.Concurrent;
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
    private static readonly ConcurrentDictionary<string, string> _connectionToUser = new();

    public async Task Join(Guid callId)
    {
        var userResult = await mediator.Send(new GetCurrentUserQuery());
        if (userResult.IsFailure)
        {
            throw new HubException(userResult.Error.Description);
        }

        var userId = userResult.Value.Id.ToString();
        var callIdStr = callId.ToString();

        if (_userConnections.TryGetValue(userId, out var oldConnectionId))
        {
            if (_userCalls.TryGetValue(userId, out var oldCallIdStr) && Guid.TryParse(oldCallIdStr, out var oldCallId))
            {
                await Groups.RemoveFromGroupAsync(oldConnectionId, $"call:{oldCallId}");
                await Clients.OthersInGroup($"call:{oldCallId}").ReceiveHangUp(userId);
                await BroadcastUserList(oldCallId);
            }

            _connectionToUser.TryRemove(oldConnectionId, out _);
            _userConnections.TryRemove(userId, out _);
            _userCalls.TryRemove(userId, out _);
        }

        _userConnections[userId] = Context.ConnectionId;
        _userCalls[userId] = callIdStr;
        _connectionToUser[Context.ConnectionId] = userId;

        await Groups.AddToGroupAsync(Context.ConnectionId, $"call:{callId}");

        await BroadcastUserList(callId);
    }

    public async Task Leave(Guid callId)
    {
        var userId = GetUserIdByConnection(Context.ConnectionId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"call:{callId}");

        if (userId != null)
        {
            _userConnections.TryRemove(userId, out _);
            _userCalls.TryRemove(userId, out _);
            _connectionToUser.TryRemove(Context.ConnectionId, out _);

            await Clients.OthersInGroup($"call:{callId}").ReceiveHangUp(userId);
        }

        await BroadcastUserList(callId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserIdByConnection(Context.ConnectionId);

        if (userId != null)
        {
            _userConnections.TryRemove(userId, out _);
            _connectionToUser.TryRemove(Context.ConnectionId, out _);

            if (_userCalls.TryRemove(userId, out var callIdStr) && Guid.TryParse(callIdStr, out var callId))
            {
                await Clients.OthersInGroup($"call:{callId}").ReceiveHangUp(userId);
                await BroadcastUserList(callId);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendVideoOffer(Guid callId, string targetUserId, string sdp)
    {
        var senderId = GetUserIdByConnection(Context.ConnectionId);
        if (senderId == null)
        {
            return;
        }

        var connection = GetUserConnection(callId, targetUserId);
        await Clients.Client(connection).ReceiveVideoOffer(senderId, sdp);
    }

    public async Task SendVideoAnswer(Guid callId, string targetUserId, string sdp)
    {
        var senderId = GetUserIdByConnection(Context.ConnectionId);
        if (senderId == null)
        {
            return;
        }

        var connection = GetUserConnection(callId, targetUserId);
        await Clients.Client(connection).ReceiveVideoAnswer(senderId, sdp);
    }

    public async Task SendIceCandidate(Guid callId, string targetUserId, string candidateJson)
    {
        var senderId = GetUserIdByConnection(Context.ConnectionId);
        if (senderId == null)
        {
            return;
        }

        var connection = GetUserConnection(callId, targetUserId);
        await Clients.Client(connection).ReceiveIceCandidate(senderId, candidateJson);
    }

    public async Task SendHangUp(Guid callId, string targetUserId)
    {
        var senderId = GetUserIdByConnection(Context.ConnectionId);
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

    private static string? GetUserIdByConnection(string connectionId)
    {
        _connectionToUser.TryGetValue(connectionId, out var userId);
        return userId;
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
