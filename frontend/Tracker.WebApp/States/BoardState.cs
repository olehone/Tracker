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

        var boardResult = await _boardService.GetByIdAsync(boardId);

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

    public async Task<bool> CreateBoardListAsync(string title)
    {
        if (_currentBoard is null)
        {
            return false;
        }

        var request = new CreateBoardListRequest
        {
            Title = title
        };

        var result = await _boardListService.CreateBoardListAsync(Board.Id, request);
        if (result.IsFailure)
        {
            return false;
        }

        ApplyListCreated(result.Value);
        return true;
    }

    public async Task<bool> UpdateBoardAsync(UpdateBoardRequest request)
    {
        if (_currentBoard is null)
        {
            return false;
        }

        var result = await _boardService.UpdateAsync(_currentBoard.Id, request);

        if (result.IsFailure)
        {
            return false;
        }

        ApplyBoardUpdated(request);
        return true;
    }

    public async Task DeleteBoardAsync()
    {
        if (_currentBoard is null)
        {
            return false;
        }

        var result = await _boardService.DeleteBoardAsync(_currentBoard.Id);
        if (result.IsFailure)
        {
            return;
        }
        OnBoardNotFound?.Invoke();
    }


    public async Task<bool> MoveBoardListAsync(Guid listId, int newPosition)
    {
        if (_currentBoard is null)
        {
            return false;
        }

        var request = new MoveBoardListRequest
        {
            Position = newPosition
        };

        var result = await _boardListService.MoveBoardListAsync(listId, request);
        if (result.IsFailure)
        {
            return false;
        }

        ApplyListMoved(listId, newPosition);
        return true;
    }

    public async Task<bool> UpdateBoardListAsync(Guid listId, UpdateBoardListRequest request)
    {
        if (_currentBoard is null)
        {
            return false;
        }

        ApplyListUpdated(listId, request);

        var result = await _boardListService.UpdateBoardListAsync(listId, request);
        if (result.IsFailure)
        {
            return false;
        }
        return true;
    }

    public async Task<bool> DeleteBoardListAsync(Guid itemId)
    {
        if (_currentBoard is null)
        {
            return false;
        }

        ApplyListDeleted(itemId);

        var result = await _boardListService.DeleteBoardListAsync(itemId);
        if (result.IsFailure)
        {
            return false;
        }
        return true;
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
