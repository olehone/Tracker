using System.Runtime.CompilerServices;
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

    public async Task CreateAsync(Guid boardListId, string title)
    {
        var request = new CreateBoardItemRequest
        {
            Title = title
        };

        var result = await boardItemService.CreateAsync(Board.Id, boardListId, request);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
            return;
        }

        ApplyCreated(result.Value);
    }

    public async Task MoveAsync(Guid itemId, string toBoardListId, int position)
    {
        if (!Guid.TryParse(toBoardListId, out Guid boardListId))
        {
            await boardState.ReloadAsync();
            return;
        }

        var request = new MoveBoardItemRequest
        {
            ToBoardListId = boardListId,
            Position = position
        };

        ApplyMoved(itemId, boardListId, position);

        var result = await boardItemService.MoveAsync(Board.Id, itemId, request);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
        }
    }

    public async Task UpdateAsync(Guid itemId, UpdateBoardItemRequest request)
    {
        ApplyUpdated(itemId, request.Title, request.Description, request.IsDone);

        var result = await boardItemService.UpdateAsync(Board.Id, itemId, request);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
        }
    }

    public async Task DeleteAsync(Guid itemId)
    {
        ApplyDeleted(itemId);

        var result = await boardItemService.DeleteAsync(Board.Id, itemId);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
        }
    }

    public async Task AssignAsync(Guid itemId, Guid userId)
    {
        var item = _boardItems.FirstOrDefault(bi => bi.Id == itemId);
        if (item is null)
        {
            return;
        }
        item.Assignees.Add(userId);
        Notify();

        var result = await boardItemService.AssignAsync(Board.Id, itemId, userId);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
        }
    }

    public async Task RemoveAsync(Guid itemId, Guid userId)
    {
        var item = _boardItems.FirstOrDefault(bi => bi.Id == itemId);
        if (item is null)
        {
            return;
        }
        item.Assignees.Remove(userId);
        Notify();

        var result = await boardItemService.RemoveAsync(Board.Id, itemId, userId);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
        }
    }

    public void Apply(ItemCreatedEvent evt)
    {
        if (boardState.MyId == evt.UserId)
        {
            return;
        }
        ApplyCreated(evt.Item);
        boardState.UsersState.MarkActivity(evt.UserId);
    }

    public void Apply(ItemMovedEvent evt)
    {
        if (boardState.MyId == evt.UserId)
        {
            return;
        }
        ApplyMoved(evt.ItemId, evt.ToListId, evt.Position);
        boardState.UsersState.MarkActivity(evt.UserId);
    }

    public void Apply(ItemUpdatedEvent evt)
    {
        if (boardState.MyId == evt.UserId)
        {
            return;
        }
        ApplyUpdated(evt.Item.Id, evt.Item.Title, evt.Item.Description, evt.Item.IsDone, evt.Item.Assignees);
        boardState.UsersState.MarkActivity(evt.UserId);
    }

    public void Apply(ItemDeletedEvent evt)
    {
        if (boardState.MyId == evt.UserId)
        {
            return;
        }
        ApplyDeleted(evt.ItemId);
        boardState.UsersState.MarkActivity(evt.UserId);
    }

    private void ApplyCreated(BoardItemDto newItem)
    {
        _boardItems.Add(newItem);
        Notify();
    }

    private void ApplyMoved(Guid itemId, Guid boardListId, int position)
    {
        var item = _boardItems.FirstOrDefault(bi => bi.Id == itemId);
        if (item is null)
        {
            return;
        }

        if (item.BoardListId == boardListId)
        {
            if (item.Position == position)
            {
                return;
            }

            if (item.Position > position)
            {
                ShiftPosition(item.BoardListId, +1, position, item.Position - 1);
                item.Position = position;
            }
            else
            {
                ShiftPosition(item.BoardListId, -1, item.Position + 1, position);
                item.Position = position;
            }
        }
        else
        {
            var oldListId = item.BoardListId;
            var oldPosition = item.Position;

            ShiftPosition(oldListId, -1, oldPosition + 1);
            ShiftPosition(boardListId, +1, position);

            item.BoardListId = boardListId;
            item.Position = position;
        }

        Notify();
    }

    private void ApplyUpdated(Guid itemId, string title, string description, bool isDone,
        HashSet<Guid>? assignees = null)
    {
        var item = _boardItems.FirstOrDefault(bi => bi.Id == itemId);
        if (item is null)
        {
            return;
        }
        item.Title = title;
        item.Description = description;
        item.IsDone = isDone;
        if (assignees is not null)
        {
            item.Assignees = assignees;
        }
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

    private void ShiftPosition(Guid listId, int delta, int from)
    {
        foreach (var item in _boardItems.Where(bi => bi.BoardListId == listId && bi.Position >= from))
        {
            item.Position += delta;
        }
    }

    private void ShiftPosition(Guid listId, int delta, int from, int to)
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