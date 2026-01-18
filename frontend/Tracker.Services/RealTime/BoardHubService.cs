using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using Tracker.Domain.Events;
using Tracker.Domain.Options;
using Tracker.Services.Abstraction;

namespace Tracker.Services.RealTime;

public class BoardHubService(
    IOptions<ApiOptions> apiOptions,
    IAuthService authService) : IBoardRealtimeService
{
    private readonly string _hubUrl = $"{apiOptions.Value.ApiBaseUrl}/hubs/board";
    private HubConnection? _hubConnection;
    private Guid? _currentBoardId;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public event Action<ItemMovedEvent>? OnItemMoved;

    public async Task ConnectAndJoinBoardAsync(Guid boardId)
    {
        if (_hubConnection != null && _currentBoardId != boardId)
        {
            await DisconnectAsync();
        }

        if (_hubConnection != null && _currentBoardId == boardId && IsConnected)
        {
            return;
        }

        var token = await authService.GetAccessTokenAsync();

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_hubUrl, options =>
            {
                if (!string.IsNullOrEmpty(token))
                {
                    options.AccessTokenProvider = () => Task.FromResult(token)!;
                }
            })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<ItemMovedEvent>(BoardRealtimeMethods.ItemMoved, (evt) =>
        {
             OnItemMoved?.Invoke(evt);
        });

        _hubConnection.Reconnected += async connectionId =>
        {
            if (_currentBoardId.HasValue)
            {
                await _hubConnection.InvokeAsync(BoardRealtimeMethods.JoinBoard, _currentBoardId.Value);
            }
        };

        _hubConnection.Closed += async (error) =>
        {
            await Task.CompletedTask;
        };

        await _hubConnection.StartAsync();
        await _hubConnection.InvokeAsync(BoardRealtimeMethods.JoinBoard, boardId);
        _currentBoardId = boardId;
    }

    public async Task DisconnectAsync()
    {
        if (_hubConnection == null)
        {
            return;
        }

        try
        {
            if (_currentBoardId.HasValue && IsConnected)
            {
                await _hubConnection.InvokeAsync(BoardRealtimeMethods.LeaveBoard, _currentBoardId.Value);
                _currentBoardId = null;
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
        GC.SuppressFinalize(this);
        await DisconnectAsync();
    }
}