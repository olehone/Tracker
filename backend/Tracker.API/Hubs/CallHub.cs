using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Tracker.API.Hubs.Interfaces;
using Tracker.Application.UseCases.Calls;
using Tracker.Application.UseCases.Calls.Disconnect;
using Tracker.Application.UseCases.Calls.GetTransferInfo;
using Tracker.Application.UseCases.Calls.Join;
using Tracker.Application.UseCases.Calls.Leave;
using Tracker.Application.UseCases.Calls.Peek;

namespace Tracker.API.Hubs;

[Authorize]
public class CallHub(IMediator mediator) : Hub<IClientCallHub>
{
    public async Task Peek(Guid callId)
    {
        var request = new PeekCallCommand
        {
            CallId = callId,
            ConnectionId = Context.ConnectionId,
        };
        var result = await mediator.Send(request);
        if (result.IsFailure)
        {
            throw new HubException(result.Error.Description);
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(callId));
    }

    public async Task Join(Guid callId)
    {
        var request = new JoinCallCommand
        {
            CallId = callId,
            ConnectionId = Context.ConnectionId,
        };
        var result = await mediator.Send(request);
        if (result.IsFailure)
        {
            throw new HubException(result.Error.Description);
        }

        await Clients.Group(GroupName(callId)).UserJoined(result.Value);
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(callId));

    }

    public async Task Leave(Guid callId)
    {
        var request = new LeaveCallCommand
        {
            CallId = callId,
        };
        var result = await mediator.Send(request);
        if (result.IsFailure)
        {
            throw new HubException(result.Error.Description);
        }

        await HandleLeaving(callId, result.Value);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var request = new DisconnectFromCallCommand
        {
            ConnectionId = Context.ConnectionId
        };
        var result = await mediator.Send(request);
        if (result.IsFailure)
        {
            throw new HubException(result.Error.Description);
        }

        await HandleLeaving(result.Value.CallId, result.Value.LeaveInfo);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendVideoOffer(Guid callId, string targetUserId, string sdp)
    {
        var info = await GetTransferInfoAsync(callId, targetUserId);
        await Clients.Client(info.ConnectionId).ReceiveVideoOffer(info.SenderId, sdp);
    }

    public async Task SendVideoAnswer(Guid callId, string targetUserId, string sdp)
    {
        var info = await GetTransferInfoAsync(callId, targetUserId);
        await Clients.Client(info.ConnectionId).ReceiveVideoAnswer(info.SenderId, sdp);
    }

    public async Task SendIceCandidate(Guid callId, string targetUserId, string candidateJson)
    {
        var info = await GetTransferInfoAsync(callId, targetUserId);
        await Clients.Client(info.ConnectionId).ReceiveIceCandidate(info.SenderId, candidateJson);
    }

    public async Task SendHangUp(Guid callId, string targetUserId)
    {
        var info = await GetTransferInfoAsync(callId, targetUserId);
        await Clients.Client(info.ConnectionId).ReceiveHangUp(info.SenderId);
    }

    private async Task<TransferInfo> GetTransferInfoAsync(Guid callId, string targetUserId)
    {
        var request = new GetTransferInfoQuery
        {
            CallId = callId,
            TargetUserId = targetUserId,
        };
        var result = await mediator.Send(request);
        if (result.IsFailure)
        {
            var message = result.Error.Details is not null && result.Error.Details.Any()
                ? result.Error.Details[0]
                : result.Error.Description;
            throw new HubException(message);
        }
        return result.Value;
    }

    private async Task HandleLeaving(Guid callId, LeaveInfo info)
    {
        if (info.CallEnded)
        {
            await Clients.Group(GroupName(callId)).CallEnded();
            foreach (var connectionId in info.ConnectionIds)
            {
                await Groups.RemoveFromGroupAsync(connectionId, GroupName(callId));
            }
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(callId));
        await Clients.Group(GroupName(callId)).UserLeaved(info.UserId);
    }

    private static string GroupName(Guid callId)
    {
        return $"call:{callId}";
    }
}