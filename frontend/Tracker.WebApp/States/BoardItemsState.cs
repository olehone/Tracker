using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.States;

public sealed class BoardItemsState
{
    private readonly BoardState _boardState;
    private readonly IBoardItemService _boardItemService;
    private readonly List<BoardItemDto> _boardItems= [];

    public IReadOnlyList<BoardItemDto> BoardItems=> _boardItems;
    public event Action? OnChange;

    private BoardFullDto Board => _boardState.Board!;

    public BoardItemsState(
        BoardState boardState,
        IBoardItemService boardItemService)
    {
        _boardState = boardState;
        _boardItemService = boardItemService;
    }

    public void Reload()
    {
        var items = Board.BoardLists.SelectMany(bl => bl.BoardItems).ToList() ?? [];
        _boardItems.Clear();
        _boardItems.AddRange(items);
    }

    public async Task<bool> CreateBoardItemAsync(Guid boardListId, string title)
    {
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
            return false;
        }

        return true;
    }

    public async Task<bool> UpdateBoardItemAsync(Guid itemId, UpdateBoardItemRequest request)
    {
        ApplyItemUpdated(itemId, request);

        var result = await _boardItemService.UpdateBoardItemAsync(itemId, request);
        if (result.IsFailure)
        {
            return false;
        }
        return true;
    }

    public async Task<bool> DeleteBoardItemAsync(Guid itemId)
    {
        ApplyItemDeleted(itemId);

        var result = await _boardItemService.DeleteBoardItemAsync(itemId);
        if (result.IsFailure)
        {
            return false;
        }
        return true;
    }

    private void ApplyItemCreated(BoardItemDto newItem)
    {
        var list = Board.BoardLists.FirstOrDefault(l => l.Id == newItem.BoardListId);
        list?.BoardItems.Add(newItem);

        Notify();
    }

    private void ApplyItemMoved(MoveBoardItemRequest request)
    {
        var item = _boardItems.FirstOrDefault(bi => bi.Id == request.BoardItemId);

        if (item is null)
        {
            return;
        }

        var fromList = Board.BoardLists.FirstOrDefault(bl => bl.Id == item.BoardListId);
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
            var toList = Board.BoardLists.FirstOrDefault(bl => bl.Id == request.ToBoardListId);
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

    private void ApplyItemUpdated(Guid itemId, UpdateBoardItemRequest request)
    {
        var item = Board.BoardLists.SelectMany(bl=> bl.BoardItems).FirstOrDefault(bi => bi.Id  == itemId);
        if (item is null)
        {
            return;
        }
        item.Title = request.Title;
        item.Description = request.Description;

        Notify();
    }

    private void ApplyItemDeleted(Guid itemId)
    {
        var list = Board.BoardLists
            .FirstOrDefault(bl => bl.BoardItems.Any(bi => bi.Id == itemId));
        if (list is null)
        {
            return;
        }

        var item = list.BoardItems.FirstOrDefault(bi => bi.Id == itemId);
        if (item is null)
        {
            return;
        }

        var deletedPosition = item.Position;

        list.BoardItems.Remove(item);
        ShiftItemsPosition(list, -1, deletedPosition + 1);
        list.BoardItems = list.BoardItems
            .OrderBy(bi => bi.Position)
            .ToList();
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

    private void Notify() => OnChange?.Invoke();
}
