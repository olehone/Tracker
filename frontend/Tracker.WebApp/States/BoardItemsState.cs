using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.States;

public sealed class BoardItemsState
{
    private readonly BoardState _boardState;
    private readonly IBoardItemService _boardItemService;
    private readonly List<BoardItemDto> _boardItems = [];

    public IReadOnlyList<BoardItemDto> BoardItems => _boardItems;
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
        var items = Board.BoardLists.SelectMany(bl => bl.BoardItems).ToList();
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
        _boardItems.Add(newItem);
        Notify();
    }

    private void ApplyItemMoved(MoveBoardItemRequest request)
    {
        var item = _boardItems.FirstOrDefault(bi => bi.Id == request.BoardItemId);
        if (item is null)
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
                ShiftItemsPosition(item.BoardListId, +1, request.Position, item.Position - 1);
                item.Position = request.Position;
            }
            else
            {
                ShiftItemsPosition(item.BoardListId, -1, item.Position + 1, request.Position);
                item.Position = request.Position;
            }
        }
        else
        {
            var oldListId = item.BoardListId;
            var oldPosition = item.Position;

            ShiftItemsPosition(oldListId, -1, oldPosition + 1);
            ShiftItemsPosition(request.ToBoardListId, +1, request.Position);

            item.BoardListId = request.ToBoardListId;
            item.Position = request.Position;
        }

        Notify();
    }

    private void ApplyItemUpdated(Guid itemId, UpdateBoardItemRequest request)
    {
        var item = _boardItems.FirstOrDefault(bi => bi.Id == itemId);
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
        var item = _boardItems.FirstOrDefault(bi => bi.Id == itemId);
        if (item is null)
        {
            return;
        }

        var deletedPosition = item.Position;
        var listId = item.BoardListId;

        _boardItems.Remove(item);

        foreach (var i in _boardItems.Where(bi => bi.BoardListId == listId && bi.Position > deletedPosition))
        {
            i.Position -= 1;
        }

        Notify();
    }

    private void ShiftItemsPosition(Guid listId, int delta, int from)
    {
        foreach (var item in _boardItems.Where(bi => bi.BoardListId == listId && bi.Position >= from))
        {
            item.Position += delta;
        }
    }

    private void ShiftItemsPosition(Guid listId, int delta, int from, int to)
    {
        foreach (var item in _boardItems.Where(bi => bi.BoardListId == listId && bi.Position >= from && bi.Position <= to))
        {
            item.Position += delta;
        }
    }

    private void Notify() => OnChange?.Invoke();
}