namespace Tracker.Services.Abstraction.Realtime;

public interface ICallRealtimeService : IRealtimeService
{
    event Action<string> OnDataSent; 

    Task SendData(string data);
}