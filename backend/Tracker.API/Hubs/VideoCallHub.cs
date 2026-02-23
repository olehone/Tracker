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
    // --- per-instance (connectionId is meaningless across servers, stays in-memory even with Redis) ---
    private static readonly ConcurrentDictionary<string, string> _connectionToUser = new();

    // --- shared state (these move to Redis when you scale) ---
    private static readonly ConcurrentDictionary<string, string> _userConnections = new();  // userId -> connectionId
    private static readonly ConcurrentDictionary<string, string> _userCalls = new();        // userId -> callId
    private static readonly ConcurrentDictionary<string, string> _userStatus = new();       // userId -> "peeking"|"active"
    private static readonly ConcurrentDictionary<string, DateTimeOffset> _callStartTimes = new(); // callId -> startedAt

    // -------------------------------------------------------------------------
    // Peek — joins the call group but stays invisible in the participant list
    // -------------------------------------------------------------------------

    public async Task Peek(Guid callId)
    {
        var userId = await GetCurrentUserIdAsync();
        var callIdStr = callId.ToString();

        // Clean up any previous registration for this user
        await RemoveUserFromCurrentCallAsync(userId, notifyOthers: false);

        _userConnections[userId] = Context.ConnectionId;
        _userCalls[userId] = callIdStr;
        _userStatus[userId] = "peeking";
        _connectionToUser[Context.ConnectionId] = userId;

        await Groups.AddToGroupAsync(Context.ConnectionId, $"call:{callId}");

        // Send current snapshot only to this caller — don't broadcast,
        // peekers don't affect anyone else's view
        var metadata = BuildCallMetadata(callId);
        await Clients.Caller.ReceiveCallMetadata(metadata.ParticipantCount, metadata.StartedAt);
    }

    // -------------------------------------------------------------------------
    // Join — becomes a full active participant, appears in user list
    // -------------------------------------------------------------------------

    public async Task Join(Guid callId)
    {
        var userId = await GetCurrentUserIdAsync();
        var callIdStr = callId.ToString();

        var wasAlreadyInThisCall = _userCalls.TryGetValue(userId, out var existingCallId)
                                   && existingCallId == callIdStr;

        if (!wasAlreadyInThisCall)
        {
            // Coming from a different call or fresh — clean up first
            await RemoveUserFromCurrentCallAsync(userId, notifyOthers: true);

            _userConnections[userId] = Context.ConnectionId;
            _userCalls[userId] = callIdStr;
            _connectionToUser[Context.ConnectionId] = userId;

            await Groups.AddToGroupAsync(Context.ConnectionId, $"call:{callId}");
        }
        else
        {
            // Was peeking this same call — connection already in the group,
            // just flip the status
            _userConnections[userId] = Context.ConnectionId;
        }

        // Record start time the first time an active user joins
        if (!_callStartTimes.ContainsKey(callIdStr))
            _callStartTimes.TryAdd(callIdStr, DateTimeOffset.UtcNow);

        _userStatus[userId] = "active";

        // Broadcast updated participant list and metadata to everyone in the call
        await BroadcastUserList(callId);
        await BroadcastCallMetadata(callId);

        var metadata = BuildCallMetadata(callId);
        await Clients.Caller.ReceiveCallMetadata(metadata.ParticipantCount, metadata.StartedAt);
    }

    // -------------------------------------------------------------------------
    // Leave
    // -------------------------------------------------------------------------

    public async Task Leave(Guid callId)
    {
        var userId = GetUserIdByConnection(Context.ConnectionId);
        if (userId == null) return;

        await RemoveUserFromCurrentCallAsync(userId, notifyOthers: true);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"call:{callId}");
        await BroadcastUserList(callId);
        await BroadcastCallMetadata(callId);
        CleanUpCallIfEmpty(callId);
    }

    // -------------------------------------------------------------------------
    // Disconnect
    // -------------------------------------------------------------------------

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserIdByConnection(Context.ConnectionId);

        if (userId != null)
        {
            if (_userCalls.TryGetValue(userId, out var callIdStr)
                && Guid.TryParse(callIdStr, out var callId))
            {
                var wasActive = _userStatus.GetValueOrDefault(userId) == "active";

                CleanUpUser(userId);

                if (wasActive)
                {
                    await Clients.OthersInGroup($"call:{callId}").ReceiveHangUp(userId);
                    await BroadcastUserList(callId);
                }

                await BroadcastCallMetadata(callId);
                CleanUpCallIfEmpty(callId);
            }
            else
            {
                CleanUpUser(userId);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    // -------------------------------------------------------------------------
    // WebRTC signalling — only meaningful between active participants
    // -------------------------------------------------------------------------

    public async Task SendVideoOffer(Guid callId, string targetUserId, string sdp)
    {
        var senderId = GetUserIdByConnection(Context.ConnectionId);
        if (senderId == null) return;

        var connection = GetActiveUserConnection(callId, targetUserId);
        await Clients.Client(connection).ReceiveVideoOffer(senderId, sdp);
    }

    public async Task SendVideoAnswer(Guid callId, string targetUserId, string sdp)
    {
        var senderId = GetUserIdByConnection(Context.ConnectionId);
        if (senderId == null) return;

        var connection = GetActiveUserConnection(callId, targetUserId);
        await Clients.Client(connection).ReceiveVideoAnswer(senderId, sdp);
    }

    public async Task SendIceCandidate(Guid callId, string targetUserId, string candidateJson)
    {
        var senderId = GetUserIdByConnection(Context.ConnectionId);
        if (senderId == null) return;

        var connection = GetActiveUserConnection(callId, targetUserId);
        await Clients.Client(connection).ReceiveIceCandidate(senderId, candidateJson);
    }

    public async Task SendHangUp(Guid callId, string targetUserId)
    {
        var senderId = GetUserIdByConnection(Context.ConnectionId);
        if (senderId == null) return;

        var connection = GetActiveUserConnection(callId, targetUserId);
        await Clients.Client(connection).ReceiveHangUp(senderId);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private async Task<string> GetCurrentUserIdAsync()
    {
        var result = await mediator.Send(new GetCurrentUserQuery());
        if (result.IsFailure)
            throw new HubException(result.Error.Description);
        return result.Value.Id.ToString();
    }

    // Removes the user from their current call cleanly.
    // notifyOthers=true sends ReceiveHangUp to other active participants.
    private async Task RemoveUserFromCurrentCallAsync(string userId, bool notifyOthers)
    {
        if (!_userCalls.TryGetValue(userId, out var callIdStr))
            return;

        var wasActive = _userStatus.GetValueOrDefault(userId) == "active";

        if (_userConnections.TryGetValue(userId, out var oldConnectionId))
        {
            await Groups.RemoveFromGroupAsync(oldConnectionId, $"call:{callIdStr}");
            _connectionToUser.TryRemove(oldConnectionId, out _);
        }

        CleanUpUser(userId);

        if (notifyOthers && wasActive && Guid.TryParse(callIdStr, out var callId))
        {
            await Clients.OthersInGroup($"call:{callId}").ReceiveHangUp(userId);
        }
    }

    private void CleanUpUser(string userId)
    {
        if (_userConnections.TryRemove(userId, out var connId))
            _connectionToUser.TryRemove(connId, out _);
        _userCalls.TryRemove(userId, out _);
        _userStatus.TryRemove(userId, out _);
    }

    private void CleanUpCallIfEmpty(Guid callId)
    {
        var callIdStr = callId.ToString();
        var hasActiveUsers = _userCalls.Any(kvp =>
            kvp.Value == callIdStr && _userStatus.GetValueOrDefault(kvp.Key) == "active");

        if (!hasActiveUsers)
            _callStartTimes.TryRemove(callIdStr, out _);
    }

    // Sends the active participant list to everyone in the group (including peekers).
    // Peekers receive it too so they can see the live count in the lobby.
    private async Task BroadcastUserList(Guid callId)
    {
        var callIdStr = callId.ToString();
        var activeUsers = _userCalls
            .Where(kvp => kvp.Value == callIdStr && _userStatus.GetValueOrDefault(kvp.Key) == "active")
            .Select(kvp => kvp.Key)
            .ToArray();

        await Clients.Group($"call:{callId}").UserListUpdated(activeUsers);
    }

    private record CallMetadataSnapshot(int ParticipantCount, DateTimeOffset? StartedAt);

    private CallMetadataSnapshot BuildCallMetadata(Guid callId)
    {
        var callIdStr = callId.ToString();
        var count = _userCalls.Count(kvp =>
            kvp.Value == callIdStr && _userStatus.GetValueOrDefault(kvp.Key) == "active");
        _callStartTimes.TryGetValue(callIdStr, out var startedAt);
        return new CallMetadataSnapshot(count, count > 0 ? startedAt : null);
    }

    private async Task BroadcastCallMetadata(Guid callId)
    {
        var metadata = BuildCallMetadata(callId);
        await Clients.Group($"call:{callId}")
            .ReceiveCallMetadata(metadata.ParticipantCount, metadata.StartedAt);
    }

    // Guards WebRTC signalling to active participants only
    private string GetActiveUserConnection(Guid callId, string targetUserId)
    {
        if (!_userCalls.TryGetValue(targetUserId, out var targetCallId))
            throw new HubException("User is not on call");

        if (targetCallId != callId.ToString())
            throw new HubException("User is on another call");

        if (_userStatus.GetValueOrDefault(targetUserId) != "active")
            throw new HubException("User is not an active participant");

        if (!_userConnections.TryGetValue(targetUserId, out var connectionId))
            throw new HubException("User connection not found");

        return connectionId;
    }

    private static string? GetUserIdByConnection(string connectionId)
    {
        _connectionToUser.TryGetValue(connectionId, out var userId);
        return userId;
    }
}
