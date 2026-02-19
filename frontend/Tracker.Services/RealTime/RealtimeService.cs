using Microsoft.AspNetCore.SignalR.Client;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Realtime;
using Tracker.Services.Realtime.Methods;

namespace Tracker.Services.Realtime;

public abstract class RealtimeService(IApiUrlService apiUrl, IAuthService authService, string endpoint)
    : IRealtimeService
{
    private readonly string _hubUrl = $"{apiUrl.GetApiUrl()}/{endpoint}";

    private HubConnection? _hubConnection;
    private Guid? _currentEntityId;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public async Task ConnectAsync(Guid entityId)
    {
        if (_hubConnection != null && _currentEntityId != entityId)
        {
            await DisconnectAsync();
        }

        if (_hubConnection != null && _currentEntityId == entityId && IsConnected)
        {
            return;
        }

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_hubUrl, options =>
            {
                options.AccessTokenProvider = authService.GetAccessTokenAsync;
            })
            .WithAutomaticReconnect()
            .Build();

        RegisterEvents(_hubConnection);

        _hubConnection.Reconnected += async connectionId =>
        {
            if (_currentEntityId.HasValue)
            {
                await _hubConnection.InvokeAsync(RealtimeMethods.Join, _currentEntityId.Value);
            }
        };

        _hubConnection.Closed += async (error) =>
        {
            await Task.CompletedTask;
        };

        await _hubConnection.StartAsync();
        await _hubConnection.InvokeAsync(RealtimeMethods.Join, entityId);
        _currentEntityId = entityId;
    }

    public async Task DisconnectAsync()
    {
        if (_hubConnection == null)
        {
            return;
        }

        try
        {
            if (_currentEntityId.HasValue && IsConnected)
            {
                await _hubConnection.InvokeAsync(RealtimeMethods.Leave, _currentEntityId.Value);
                _currentEntityId = null;
            }

            await _hubConnection.StopAsync();
        }
        finally
        {
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }
    }

    public virtual async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
    }

    public abstract void RegisterEvents(HubConnection hubConnection);
}