using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Domain.Requests.BoardList;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.States;

public sealed class BoardState
{
    private readonly IBoardService _boardService;
    private readonly IBoardListService _boardListService;

    private BoardFullDto? _currentBoard;

    public BoardFullDto Board => _currentBoard
        ?? throw new InvalidOperationException("BoardState accessed before board was loaded.");
    public BoardUsersState Users { get; }
    public BoardItemsState Items { get; }
    public BoardListsState Lists { get; }

    public bool IsLoading { get; private set; }
    public event Action? OnChange;
    public event Action? OnBoardNotFound;

    public BoardState(
        IBoardService boardService,
        IBoardListService boardListService,
        IBoardItemService boardItemService,
        IBoardUserService boardUserService,
        IUserService userService)
    {
        _boardService = boardService;
        _boardListService = boardListService;

        Users = new BoardUsersState(this, boardUserService, userService);
        Items = new BoardItemsState(this, boardItemService);
        Lists = new BoardListsState(this, boardListService);
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
            await Users.LoadAsync();
            Items.Reload();
            Lists.Reload();
        }

        IsLoading = false;
        Notify();
    }

    public Task ReloadAsync()
    {
        return LoadAsync(Board.Id);
    }

    public async Task<bool> UpdateBoardAsync(UpdateBoardRequest request)
    {
        if (_currentBoard is null)
        {
            return false;
        }

        var result = await _boardService.UpdateBoardAsync(_currentBoard.Id, request);

        if (result.IsFailure)
        {
            return false;
        }

        ApplyBoardUpdated(request);
        return true;
    }

    public async Task DeleteBoardAsync()
    {
        var result = await _boardService.DeleteBoardAsync(Board.Id);
        if (result.IsFailure)
        {
            return;
        }
        OnBoardNotFound?.Invoke();
    }

    private void ApplyBoardUpdated(UpdateBoardRequest request)
    {
        if (_currentBoard is null)
        {
            return;
        }

        _currentBoard.Title = request.Title;
        _currentBoard.Description = request.Description;
        _currentBoard.Visibility = request.Visibility;
        _currentBoard.PermissionRoles = request.PermissionRoles;

        Notify();
    }

    private void Notify() => OnChange?.Invoke();
}
