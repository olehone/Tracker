namespace Tracker.Services.Abstraction.Realtime;

public interface IRealtimeService : IAsyncDisposable
{
    Task ConnectAsync(Guid entityId);
    Task DisconnectAsync();
    bool IsConnected { get; }
}
