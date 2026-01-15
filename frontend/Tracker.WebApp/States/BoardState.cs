using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Domain.Requests.BoardItem;
using Tracker.Domain.Requests.BoardList;
using Tracker.Services.Abstraction;
using Tracker.WebApp.Shared;

namespace Tracker.WebApp.States;

public sealed class BoardState
{
    private readonly IBoardService _boardService;
    private readonly IBoardListService _boardListService;
    private readonly IBoardItemService _boardItemService;

    private BoardFullDto? _currentBoard;

    public BoardFullDto Board => _currentBoard
        ?? throw new InvalidOperationException("BoardState accessed before board was loaded.");
    public BoardUsersState Users { get; }

    public bool IsLoading { get; private set; }
    public event Action? OnChange;

    public BoardState(
        IBoardService boardService,
        IBoardListService boardListService,
        IBoardItemService boardItemService,
        IBoardUserService boardUserService,
        IUserService userService)
    {
        _boardService = boardService;
        _boardListService = boardListService;
        _boardItemService = boardItemService;

        Users = new BoardUsersState(this, boardUserService, userService);
    }

    public async Task LoadAsync(Guid boardId)
    {
        IsLoading = true;
        Notify();

        var boardResult = await _boardService.GetBoardByIdAsync(boardId);

        if (boardResult.IsFailure)
        {
            _currentBoard = null;
        }
        else
        {
            _currentBoard = boardResult.Value;
            await Users.LoadAsync();
        }

        IsLoading = false;
        Notify();
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

    public async Task<bool> DeleteBoardListAsync(Guid listId)
    {
        if (_currentBoard is null)
        {
            return false;
        }

        ApplyListDeleted(listId);

        var result = await _boardListService.DeleteBoardListAsync(listId);
        if (result.IsFailure)
        {
            return false;
        }
        return true;
    }

    public async Task<bool> CreateBoardItemAsync(Guid boardListId, string title)
    {
        if (_currentBoard is null)
        {
            return false;
        }

        var request = new CreateBoardItemRequest
        {
            Title = title
        };

        var result = await _boardItemService.CreateBoardItemAsync(boardListId, request);
        if (result.IsFailure)
        {
            return false;
        }

        ApplyItemCreated(result.Value);
        return true;
    }

    public async Task<bool> MoveBoardItemAsync(Guid itemId, string toBoardListId, int position)
    {
        if (_currentBoard is null)
        {
            return false;
        }

        var request = new MoveBoardItemRequest
        {
            BoardItemId = itemId,
            ToBoardListId = Guid.Parse(toBoardListId),
            Position = position
        };

        ApplyItemMoved(request);

        var result = await _boardItemService.MoveBoardItemAsync(request);
        if (result.IsFailure)
        {
            await LoadAsync(_currentBoard.Id);
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

    private void ApplyListCreated(BoardListDto newList)
    {
        if (_currentBoard is null)
        {
            return;
        }

        _currentBoard.BoardLists.Add(newList);
        _currentBoard.BoardLists = _currentBoard.BoardLists
            .OrderBy(bl => bl.Position)
            .ToList();

        Notify();

    }

    private void ApplyListMoved(Guid listId, int newPosition)
    {
        if (_currentBoard is null)
        {
            return;
        }

        var list = _currentBoard.BoardLists.FirstOrDefault(l => l.Id == listId);
        if (list is null)
        {
            return;
        }

        ShiftLists(_currentBoard, newPosition, list.Position);
        list.Position = newPosition;
        _currentBoard.BoardLists = _currentBoard.BoardLists
            .OrderBy(bl => bl.Position)
            .ToList();

        Notify();
    }


    private void ApplyListUpdated(Guid listId, UpdateBoardListRequest request)
    {
        var list = Board.BoardLists.FirstOrDefault(bl => bl.Id == listId);
        if (list is null)
        {
            return;
        }
        list.Title = request.Title;
        list.Description = request.Description;
        Notify();
    }

    private void ApplyListDeleted(Guid listId)
    {
        var removed = Board.BoardLists.RemoveAll(bl => bl.Id == listId);
        if (removed == 0)
        {
            return;
        }

        Notify();
    }

    private void ApplyItemCreated(BoardItemDto newItem)
    {
        if (_currentBoard is null)
        {
            return;
        }

        var list = _currentBoard.BoardLists.FirstOrDefault(l => l.Id == newItem.BoardListId);
        list?.BoardItems.Add(newItem);

        Notify();
    }

    private void ApplyItemMoved(MoveBoardItemRequest request)
    {
        if (_currentBoard is null)
        {
            return;
        }

        var item = _currentBoard.BoardLists
            .SelectMany(bl => bl.BoardItems)
            .FirstOrDefault(bi => bi.Id == request.BoardItemId);

        if (item is null)
        {
            return;
        }

        var fromList = _currentBoard.BoardLists.FirstOrDefault(bl => bl.Id == item.BoardListId);
        if (fromList is null)
        {
            return;
        }
        if (item.BoardListId == request.ToBoardListId)
        {
            if (item.Position == request.Position)
            {
                return;
            }

            if (item.Position > request.Position)
            {
                ShiftItemsPosition(fromList, +1, request.Position, item.Position - 1);
                item.Position = request.Position;
            }
            else
            {
                ShiftItemsPosition(fromList, -1, item.Position + 1, request.Position);
                item.Position = request.Position;
            }
            fromList.BoardItems = fromList.BoardItems.OrderBy(bi => bi.Position).ToList();
        }
        else
        {
            var toList = _currentBoard.BoardLists.FirstOrDefault(bl => bl.Id == request.ToBoardListId);
            if (toList is null)
            {
                return;
            }

            fromList.BoardItems.Remove(item);
            ShiftItemsPosition(fromList, -1, item.Position);
            ShiftItemsPosition(toList, +1, request.Position);

            item.BoardListId = request.ToBoardListId;
            item.Position = request.Position;
            toList.BoardItems.Insert(item.Position - 1, item);
        }

        Notify();
    }

    private static void ShiftItemsPosition(BoardListDto list, int delta, int from)
    {
        foreach (var item in list.BoardItems.Where(bi => bi.Position >= from))
        {
            item.Position += delta;
        }
    }

    private static void ShiftItemsPosition(BoardListDto list, int delta, int from, int to)
    {
        foreach (var item in list.BoardItems.Where(bi => bi.Position >= from && bi.Position <= to))
        {
            item.Position += delta;
        }
    }

    private static void ShiftLists(BoardFullDto board, int newPosition, int oldPosition)
    {
        foreach (var l in board.BoardLists)
        {
            if (oldPosition < newPosition)
            {
                if (l.Position > oldPosition && l.Position <= newPosition)
                {
                    l.Position -= 1;
                }
            }
            else
            {
                if (l.Position >= newPosition && l.Position < oldPosition)
                {
                    l.Position += 1;
                }
            }
        }
    }

    private void Notify() => OnChange?.Invoke();
}
