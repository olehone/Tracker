using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.States;

public sealed class BoardState : IAsyncDisposable
{
    private readonly IBoardService _boardService;
    private readonly AppState _appState;
    private readonly IBoardRealtimeService _boardRealtime;
    private BoardFullDto? _currentBoard;

    public bool IsUnauthenticated => _appState.IsUnauthenticated;
    public Guid MyId => _appState.MyId;

    public BoardFullDto Board => _currentBoard
        ?? throw new InvalidOperationException("BoardState accessed before board was loaded.");

    public BoardUsersState UsersState { get; }
    public BoardItemsState ItemsState { get; }
    public BoardListsState ListsState { get; }

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
        _boardService = boardService;
        _appState = appState;

        UsersState = new BoardUsersState(this, userService, boardUserService);
        ItemsState = new BoardItemsState(this, boardItemService);
        ListsState = new BoardListsState(this, boardListService);
        _boardRealtime = boardRealtime;
    }

    public async Task LoadAsync(Guid boardId)
    {
        IsLoading = true;
        Notify();

        var boardResult = await _boardService.GetByIdAsync(boardId);

        if (boardResult.IsFailure)
        {
            OnBoardNotFound?.Invoke();
        }
        else
        {
            _currentBoard = boardResult.Value;
            UsersState.Reload();
            ItemsState.Reload();
            ListsState.Reload();
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
        if (IsUnauthenticated)
        {
            return;
        }

        await _boardRealtime.ConnectAndJoinBoardAsync(Board.Id);
        _boardRealtime.OnItemCreated += ItemsState.Apply;
        _boardRealtime.OnItemMoved += ItemsState.Apply;
        _boardRealtime.OnItemUpdated += ItemsState.Apply;
        _boardRealtime.OnItemDeleted += ItemsState.Apply;

        _boardRealtime.OnListCreated += ListsState.Apply;
        _boardRealtime.OnListMoved += ListsState.Apply;
        _boardRealtime.OnListUpdated += ListsState.Apply;
        _boardRealtime.OnListDeleted += ListsState.Apply;
    }

    public async Task UpdateBoardAsync(UpdateBoardRequest request)
    {
        var result = await _boardService.UpdateAsync(Board.Id, request);

        if (result.IsFailure)
        {
            await ReloadAsync();
        }
        ApplyBoardUpdated(request);
    }

    public async Task DeleteBoardAsync()
    {
        var result = await _boardService.DeleteAsync(Board.Id);
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

    public bool IsMyId(Guid checkedId)
    {
        if (_appState.IsUnauthenticated)
        {
            return false;
        }
        return checkedId == MyId;
    }

    public async ValueTask DisposeAsync()
    {
        _boardRealtime.OnItemCreated -= ItemsState.Apply;
        _boardRealtime.OnItemMoved -= ItemsState.Apply;
        _boardRealtime.OnItemUpdated -= ItemsState.Apply;
        _boardRealtime.OnItemDeleted -= ItemsState.Apply;

        _boardRealtime.OnListCreated -= ListsState.Apply;
        _boardRealtime.OnListMoved -= ListsState.Apply;
        _boardRealtime.OnListUpdated -= ListsState.Apply;
        _boardRealtime.OnListDeleted -= ListsState.Apply;
        await _boardRealtime.DisconnectAsync();
    }

    private void Notify() => OnChange?.Invoke();
}