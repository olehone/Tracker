using Tracker.Services.Abstraction.Realtime.Events;

namespace Tracker.Services.Abstraction.Realtime;

public interface ICallRealtimeService : IRealtimeService
{
    event Action<string> OnDataReceived;
    event Action<VideoOfferEvent>? OnVideoOffer;

    Task SendData(string data);
    Task SendVideoOffer(VideoOfferEvent evt);
}