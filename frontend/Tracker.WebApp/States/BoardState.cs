using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.States;

public sealed class BoardState : IAsyncDisposable
{
    private readonly IBoardService _boardService;
    private readonly AppState _appState;
    private BoardFullDto? _currentBoard;
    private readonly IBoardRealtimeService _boardRealtime;

    public Guid? MyId => _appState.CurrentUser?.Id;
    public BoardFullDto Board => _currentBoard
        ?? throw new InvalidOperationException("BoardState accessed before board was loaded.");

    public BoardUsersState Users { get; }
    public BoardItemsState Items { get; }
    public BoardListsState Lists { get; }

    public bool IsLoading { get; private set; }
    public event Action? OnChange;
    public event Action? OnBoardNotFound;

    public BoardState(
        AppState appState,
        IBoardService boardService,
        IBoardListService boardListService,
        IBoardItemService boardItemService,
        IBoardUserService boardUserService,
        IUserService userService,
        IBoardRealtimeService boardRealtime)
    {
        _appState = appState;
        _boardService = boardService;

        Users = new BoardUsersState(this, userService, boardUserService);
        Items = new BoardItemsState(this, boardItemService);
        Lists = new BoardListsState(this, boardListService);
        _boardRealtime = boardRealtime;
    }

    public async Task LoadAsync(Guid boardId)
    {
        IsLoading = true;
        Notify();

        var boardResult = await _boardService.GetBoardByIdAsync(boardId);

        if (boardResult.IsFailure)
        {
            OnBoardNotFound?.Invoke();
        }
        else
        {
            _currentBoard = boardResult.Value;
            Users.Reload();
            Items.Reload();
            Lists.Reload();
            await ConnectRealtimeAsync();
        }

        IsLoading = false;
        Notify();
    }

    public Task ReloadAsync()
    {
        return LoadAsync(Board.Id);
    }

    public async Task ConnectRealtimeAsync()
    {
        if (_appState.CurrentUser is null)
        {
            return;
        }

        await _boardRealtime.ConnectAndJoinBoardAsync(Board.Id);
        _boardRealtime.OnItemMoved += Items.Apply;
    }

    public async Task UpdateBoardAsync(UpdateBoardRequest request)
    {
        var result = await _boardService.UpdateBoardAsync(Board.Id, request);

        if (result.IsFailure)
        {
            await ReloadAsync();
        }
        ApplyBoardUpdated(request);
    }

    public async Task DeleteBoardAsync()
    {
        var result = await _boardService.DeleteBoardAsync(Board.Id);
        if (result.IsSuccess)
        {
            OnBoardNotFound?.Invoke();
        }
    }

    private void ApplyBoardUpdated(UpdateBoardRequest request)
    {
        Board.Title = request.Title;
        Board.Description = request.Description;
        Board.Visibility = request.Visibility;
        Board.PermissionRoles = request.PermissionRoles;

        Notify();
    }

    private void Notify() => OnChange?.Invoke();

    public async ValueTask DisposeAsync()
    {
        _boardRealtime.OnItemMoved -= Items.Apply;
        await _boardRealtime.DisconnectAsync();
    }
}
