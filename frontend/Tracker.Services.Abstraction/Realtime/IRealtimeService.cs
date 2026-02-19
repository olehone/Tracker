namespace Tracker.Services.Abstraction.Realtime;

public interface IRealtimeService : IAsyncDisposable
{
    Task ConnectAsync(Guid itemId);
    Task DisconnectAsync();
    bool IsConnected { get; }
}
