using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Domain.Requests.BoardItem;
using Tracker.Domain.Requests.BoardList;
using Tracker.Services.Abstraction;
using Tracker.WebApp.Shared;

namespace Tracker.WebApp.States;

public class BoardState(
        IBoardService boardService,
        IBoardListService boardListService,
        IBoardItemService boardItemService,
        IResultNotifier notifier)
{

    private BoardFullDto? _currentBoard;
    private void NotifyStateChanged() => OnChange?.Invoke();

    public event Action? OnChange;

    public BoardFullDto? CurrentBoard => _currentBoard;
    public bool IsLoading { get; private set; }

    public async Task LoadBoardAsync(Guid boardId)
    {
        IsLoading = true;
        NotifyStateChanged();

        var result = await boardService.GetBoardByIdAsync(boardId);
        if (result.IsSuccess)
        {
            _currentBoard = result.Value;
        }
        else
        {
            _currentBoard = null;
        }

        IsLoading = false;
        NotifyStateChanged();
    }

    public async Task<bool> UpdateBoardAsync(UpdateBoardRequest request)
    {
        if (_currentBoard is null)
        {
            return false;
        }

        var result = await boardService.UpdateAsync(_currentBoard.Id, request);
        notifier.Notify(result);

        if (result.IsSuccess)
        {
            ApplyBoardUpdated(request);
            return true;
        }

        return false;
    }

    public async Task<bool> CreateBoardListAsync(string title)
    {
        if (_currentBoard is null)
        {
            return false;
        }

        var request = new CreateBoardListRequest
        {
            BoardId = _currentBoard.Id,
            Title = title
        };

        var result = await boardListService.CreateBoardListAsync(request);

        if (result.IsSuccess)
        {
            ApplyListCreated(result.Value);
            return true;
        }

        return false;
    }

    public async Task<bool> MoveBoardListAsync(Guid listId, int newPosition)
    {
        if (_currentBoard is null)
        {
            return false;
        }

        var request = new MoveBoardListRequest
        {
            BoardListId = listId,
            Position = newPosition
        };

        var result = await boardListService.MoveBoardListAsync(request);

        if (result.IsSuccess)
        {
            ApplyListMoved(listId, newPosition);
            return true;
        }

        return false;
    }

    public async Task<bool> CreateBoardItemAsync(Guid boardListId, string title)
    {
        if (_currentBoard is null)
        {
            return false;
        }

        var request = new CreateBoardItemRequest
        {
            BoardListId = boardListId,
            Title = title
        };

        var result = await boardItemService.CreateBoardItemAsync(request);

        if (result.IsSuccess)
        {
            ApplyItemCreated(result.Value);
            return true;
        }

        return false;
    }

    public async Task<bool> MoveBoardItemAsync(string itemId, string toBoardListId, int position)
    {
        if (_currentBoard is null)
        {
            return false;
        }

        var request = new MoveBoardItemRequest
        {
            BoardItemId = Guid.Parse(itemId),
            ToBoardListId = Guid.Parse(toBoardListId),
            Position = position
        };

        ApplyItemMoved(request);

        var result = await boardItemService.MoveBoardItemAsync(request);

        if (result.IsFailure)
        {
            await LoadBoardAsync(_currentBoard.Id);
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

        NotifyStateChanged();
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

        NotifyStateChanged();

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

        NotifyStateChanged();
    }

    private void ApplyItemCreated(BoardItemDto newItem)
    {
        if (_currentBoard is null)
        {
            return;
        }

        var list = _currentBoard.BoardLists.FirstOrDefault(l => l.Id == newItem.BoardListId);
        list?.BoardItems.Add(newItem);

        NotifyStateChanged();
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

        NotifyStateChanged();
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
}
