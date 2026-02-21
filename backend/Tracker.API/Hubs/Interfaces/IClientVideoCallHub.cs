using Tracker.API.Hubs.Events;

namespace Tracker.API.Hubs.Interfaces;

public interface IClientVideoCallHub
{
    Task DataSent(string data);
    Task SendVideoOffer(VideoOfferEvent evt);
}