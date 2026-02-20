namespace Tracker.Services.Abstraction.Realtime;

public interface ICallRealtimeService : IRealtimeService
{
    event Action<string> OnDataReceived; 

    Task SendData(string data);
}