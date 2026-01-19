using Tracker.Domain.Dtos;
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

    public async Task CreateAsync(Guid boardListId, string title)
    {
        var request = new CreateBoardItemRequest
        {
            Title = title
        };

        var result = await boardItemService.CreateAsync(boardListId, request);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
            return;
        }

        ApplyCreated(result.Value);
    }

    public async Task MoveAsync(Guid itemId, string toBoardListId, int position)
    {
        var request = new MoveBoardItemRequest
        {
            BoardItemId = itemId,
            ToBoardListId = Guid.Parse(toBoardListId),
            Position = position
        };

        ApplyMoved(request);

        var result = await boardItemService.MoveAsync(request);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
        }
    }

    public async Task UpdateAsync(Guid itemId, UpdateBoardItemRequest request)
    {
        ApplyUpdated(itemId, request);

        var result = await boardItemService.UpdateAsync(itemId, request);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
        }
    }

    public async Task DeleteAsync(Guid itemId)
    {
        ApplyDeleted(itemId);

        var result = await boardItemService.DeleteAsync(itemId);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
        }
    }

    private void ApplyCreated(BoardItemDto newItem)
    {
        _boardItems.Add(newItem);
        Notify();
    }

    private void ApplyMoved(MoveBoardItemRequest request)
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
                ShiftIPosition(item.BoardListId, +1, request.Position, item.Position - 1);
                item.Position = request.Position;
            }
            else
            {
                ShiftIPosition(item.BoardListId, -1, item.Position + 1, request.Position);
                item.Position = request.Position;
            }
        }
        else
        {
            var oldListId = item.BoardListId;
            var oldPosition = item.Position;

            ShiftIPosition(oldListId, -1, oldPosition + 1);
            ShiftIPosition(request.ToBoardListId, +1, request.Position);

            item.BoardListId = request.ToBoardListId;
            item.Position = request.Position;
        }

        Notify();
    }

    private void ApplyUpdated(Guid itemId, UpdateBoardItemRequest request)
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

    private void ApplyDeleted(Guid itemId)
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

    private void ShiftIPosition(Guid listId, int delta, int from)
    {
        foreach (var item in _boardItems.Where(bi => bi.BoardListId == listId && bi.Position >= from))
        {
            item.Position += delta;
        }
    }

    private void ShiftIPosition(Guid listId, int delta, int from, int to)
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