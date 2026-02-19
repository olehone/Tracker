namespace Tracker.API.Hubs.Interfaces;

public interface IClientVideoCallHub
{
    Task SendData(string data);
}