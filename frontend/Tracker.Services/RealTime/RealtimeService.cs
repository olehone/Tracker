using Microsoft.AspNetCore.SignalR.Client;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Realtime;
using Tracker.Services.Realtime.Methods;

namespace Tracker.Services.Realtime;

public abstract class RealtimeService(IApiUrlService apiUrl, IAuthService authService, string endpoint)
    : IRealtimeService
{
    private readonly string _hubUrl = $"{apiUrl.GetApiUrl()}/{endpoint}";

    protected HubConnection? _hubConnection;
    private Guid? _currentEntityId;

    protected HubConnection Connection => _hubConnection!;
    protected Guid EntityId => _currentEntityId!.Value;
    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    // Builds and starts the transport without invoking any hub method.
    // Used when the first hub call should be something other than Join (e.g. Peek).
    protected async Task StartConnectionAsync()
    {
        if (IsConnected)
            return;

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
            Console.WriteLine($"Try to reconnect with {connectionId}");
            if (_currentEntityId.HasValue)
            {
                await _hubConnection.InvokeAsync(RealtimeMethods.Join, _currentEntityId.Value);
            }
        };

        _hubConnection.Closed += async error =>
        {
            Console.WriteLine($"Closed connection, error {error}");
            await Task.CompletedTask;
        };

        await _hubConnection.StartAsync();
    }

    public async Task ConnectAsync(Guid entityId)
    {
        Console.WriteLine($"Connect to entity {entityId}");

        if (_hubConnection != null && _currentEntityId != entityId)
        {
            Console.WriteLine($"Disconnect from {_currentEntityId}");
            await DisconnectAsync();
        }

        if (_hubConnection != null && _currentEntityId == entityId && IsConnected)
        {
            Console.WriteLine($"Same id {entityId}, connected");
            return;
        }

        await StartConnectionAsync();

        await _hubConnection!.InvokeAsync(RealtimeMethods.Join, entityId);
        _currentEntityId = entityId;
        Console.WriteLine($"Joined to {entityId}");
    }

    public async Task DisconnectAsync()
    {
        if (_hubConnection == null)
        {
            Console.WriteLine("Tried to disconnect without connection");
            return;
        }

        try
        {
            if (_currentEntityId.HasValue && IsConnected)
            {
                Console.WriteLine($"Leave {_currentEntityId}");
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
