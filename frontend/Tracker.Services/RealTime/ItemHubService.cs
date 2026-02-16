using Microsoft.AspNetCore.SignalR.Client;
using Tracker.Domain.Events;
using Tracker.Services.Abstraction;

namespace Tracker.Services.RealTime;

public class ItemHubService(
    IApiUrlService apiUrl,
    IAuthService authService) : IItemRealtimeService
{

    private readonly string _hubUrl = $"{apiUrl.GetApiUrl()}/hubs/item";
    private HubConnection? _hubConnection;
    private Guid? _currentItemId;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public event Action<CommentCreatedEvent>? OnCommentCreated;
    public event Action<CommentUpdatedEvent>? OnCommentUpdated;
    public event Action<CommentDeletedEvent>? OnCommentDeleted;

    public async Task ConnectAndJoinItemAsync(Guid itemId)
    {
        if (_hubConnection != null && _currentItemId != itemId)
        {
            await DisconnectAsync();
        }

        if (_hubConnection != null && _currentItemId == itemId && IsConnected)
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

        RegisterCommentEvents();

        _hubConnection.Reconnected += async connectionId =>
        {
            if (_currentItemId.HasValue)
            {
                await _hubConnection.InvokeAsync(ItemRealtimeMethods.JoinItem, _currentItemId.Value);
            }
        };

        _hubConnection.Closed += async (error) =>
        {
            await Task.CompletedTask;
        };

        await _hubConnection.StartAsync();
        await _hubConnection.InvokeAsync(ItemRealtimeMethods.JoinItem, itemId);
        _currentItemId = itemId;
    }


    public async Task DisconnectAsync()
    {
        if (_hubConnection == null)
        {
            return;
        }

        try
        {
            if (_currentItemId.HasValue && IsConnected)
            {
                await _hubConnection.InvokeAsync(ItemRealtimeMethods.LeaveItem, _currentItemId.Value);
                _currentItemId = null;
            }

            await _hubConnection.StopAsync();
        }
        finally
        {
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
    }

    private void RegisterCommentEvents()
    {
        _hubConnection!.On<CommentCreatedEvent>(ItemRealtimeMethods.CommentCreated, (evt) =>
        {
            OnCommentCreated?.Invoke(evt);
        });

        _hubConnection!.On<CommentUpdatedEvent>(ItemRealtimeMethods.CommentUpdated, (evt) =>
        {
            OnCommentUpdated?.Invoke(evt);
        });

        _hubConnection!.On<CommentDeletedEvent>(ItemRealtimeMethods.CommentDeleted, (evt) =>
        {
            OnCommentDeleted?.Invoke(evt);
        });
    }
}