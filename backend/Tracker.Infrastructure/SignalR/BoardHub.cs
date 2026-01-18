using Microsoft.AspNetCore.SignalR;
namespace Tracker.Infrastructure.SignalR;

public abstract class BoardHub : Hub<IClientBoardHub>
{
}