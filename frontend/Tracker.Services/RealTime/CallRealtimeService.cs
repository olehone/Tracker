using Microsoft.AspNetCore.SignalR.Client;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Realtime;
using Tracker.Services.Realtime.Methods;

namespace Tracker.Services.Realtime;

public class CallRealtimeService(IApiUrlService apiUrl,IAuthService authService) 
    : RealtimeService(apiUrl, authService, "hubs/call"), ICallRealtimeService
{
    public event Action<string> OnDataSent;

    public Task SendData(string data)
    {
        throw new NotImplementedException();
    }

    public override ValueTask DisposeAsync()
    {
        return base.DisposeAsync();
    }

    public override void RegisterEvents(HubConnection connection)
    {
        connection.On<string>(CallRealtimeMethods.DataSent, (evt) =>
        {

        })
    }
}