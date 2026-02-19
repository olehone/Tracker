namespace Tracker.API.Hubs;

public interface IClientVideoCallHub
{
    Task SendData(string data);
}