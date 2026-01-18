using Tracker.Domain.Dtos;
using Tracker.Domain.Events;
using Tracker.Domain.Requests.BoardItem;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.States;

public sealed class BoardItemsState(
    BoardState boardState,
    IBoardItemService boardItemService)
{
    private readonly List<BoardItemDto> _boardItems = [];
    private List<BoardItemDto>? _sortedItems;

    public IReadOnlyList<BoardItemDto> BoardItems
    {
        get
        {
            _sortedItems ??= _boardItems
                .OrderBy(i => i.BoardListId)
                .ThenBy(i => i.Position)
                .ToList();

            return _sortedItems;
        }
    }

    public event Action? OnChange;

    private BoardFullDto Board => boardState.Board!;

    public void Reload()
    {
        var items = Board.BoardLists.SelectMany(bl => bl.BoardItems).ToList();
        _sortedItems = null;
        _boardItems.Clear();
        _boardItems.AddRange(items);
    }

    public async Task CreateBoardItemAsync(Guid boardListId, string title)
    {
        var request = new CreateBoardItemRequest
        {
            Title = title
        };

        var result = await boardItemService.CreateBoardItemAsync(boardListId, request);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
            return;
        }

        ApplyItemCreated(result.Value);
    }

    public async Task MoveBoardItemAsync(Guid itemId, string toBoardListId, int position)
    {
        if (!Guid.TryParse(toBoardListId, out Guid boardListId))
        {
            await boardState.ReloadAsync();
            return;
        }

        var request = new MoveBoardItemRequest
        {
            BoardItemId = itemId,
            ToBoardListId = boardListId,
            Position = position
        };

        ApplyItemMoved(itemId, boardListId, position);

        var result = await boardItemService.MoveBoardItemAsync(request);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
        }
    }

    public async Task UpdateBoardItemAsync(Guid itemId, UpdateBoardItemRequest request)
    {
        ApplyItemUpdated(itemId, request);

        var result = await boardItemService.UpdateBoardItemAsync(itemId, request);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
        }
    }

    public async Task DeleteBoardItemAsync(Guid itemId)
    {
        ApplyItemDeleted(itemId);

        var result = await boardItemService.DeleteBoardItemAsync(itemId);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
        }
    }

    private void ApplyItemCreated(BoardItemDto newItem)
    {
        _boardItems.Add(newItem);
        Notify();
    }

    public void Apply(ItemMovedEvent evn)
    {
        if (boardState.MyId == evn.UserId)
        {
            return;
        }
        ApplyItemMoved(evn.BoardItemId, evn.ToBoardListId, evn.Position);
    }

    private void ApplyItemMoved(Guid boardItemId, Guid toBoardListId, int position)
    {
        var item = _boardItems.FirstOrDefault(bi => bi.Id == boardItemId);
        if (item is null)
        {
            return;
        }

        if (item.BoardListId == toBoardListId)
        {
            if (item.Position == position)
            {
                return;
            }

            if (item.Position > position)
            {
                ShiftItemsPosition(item.BoardListId, +1, position, item.Position - 1);
                item.Position = position;
            }
            else
            {
                ShiftItemsPosition(item.BoardListId, -1, item.Position + 1, position);
                item.Position = position;
            }
        }
        else
        {
            var oldListId = item.BoardListId;
            var oldPosition = item.Position;

            ShiftItemsPosition(oldListId, -1, oldPosition + 1);
            ShiftItemsPosition(toBoardListId, +1, position);

            item.BoardListId = toBoardListId;
            item.Position = position;
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

    private void Notify()
    {
        _sortedItems = null;
        OnChange?.Invoke();
    }
}