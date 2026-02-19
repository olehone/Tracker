namespace Tracker.Services.Abstraction.Realtime;

public interface ICallRealtimeService : IAsyncDisposable
{
    Task ConnectAsync(Guid callId);
    Task DisconnectAsync();
    bool IsConnected { get; }
}