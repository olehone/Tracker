namespace Tracker.API.Hubs.Interfaces;

public interface IClientVideoCallHub
{
    Task DataSent(string data);
}