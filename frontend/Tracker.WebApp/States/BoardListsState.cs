using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardList;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.States;

public sealed class BoardListsState(
    BoardState boardState,
    IBoardListService boardListService)
{
    private readonly List<BoardListDto> _boardLists = [];
    private List<BoardListDto>? _sortedLists;

    public IReadOnlyList<BoardListDto> BoardLists
    {
        get
        {
            _sortedLists ??= _boardLists.OrderBy(bl => bl.Position).ToList();
            return _sortedLists;
        }
    }
    public event Action? OnChange;

    private BoardFullDto Board => boardState.Board!;

    public void Reload()
    {
        var lists = Board.BoardLists;
        _sortedLists = null;
        _boardLists.Clear();
        _boardLists.AddRange(lists);
    }

    public async Task CreateAsync(string title)
    {
        var request = new CreateBoardListRequest
        {
            Title = title
        };

        var result = await boardListService.CreateAsync(Board.Id, request);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
            return;
        }

        ApplyCreated(result.Value);
    }

    public async Task MoveAsync(Guid listId, int newPosition)
    {
        var request = new MoveBoardListRequest
        {
            Position = newPosition
        };

        ApplyMoved(listId, newPosition);

        var result = await boardListService.MoveAsync(listId, request);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
        }
    }

    public async Task UpdateAsync(Guid listId, UpdateBoardListRequest request)
    {
        ApplyUpdated(listId, request);

        var result = await boardListService.UpdateAsync(listId, request);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
        }
    }

    public async Task DeleteAsync(Guid listId)
    {
        ApplyDeleted(listId);

        var result = await boardListService.DeleteAsync(listId);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
        }
    }

    private void ApplyCreated(BoardListDto newList)
    {
        _boardLists.Add(newList);
        _boardLists.Sort((a, b) => a.Position.CompareTo(b.Position));
        Notify();
    }

    private void ApplyMoved(Guid listId, int newPosition)
    {
        var list = _boardLists.FirstOrDefault(l => l.Id == listId);
        if (list is null)
        {
            return;
        }

        var oldPosition = list.Position;
        if (newPosition == oldPosition)
        {
            return;
        }

        if (oldPosition < newPosition)
        {
            foreach (var l in _boardLists.Where(bl => bl.Position > oldPosition && bl.Position <= newPosition))
            {
                l.Position -= 1;
            }
        }
        else
        {
            foreach (var l in _boardLists.Where(bl => bl.Position >= newPosition && bl.Position < oldPosition))
            {
                l.Position += 1;
            }
        }

        list.Position = newPosition;
        _boardLists.Sort((a, b) => a.Position.CompareTo(b.Position));
        Notify();
    }

    private void ApplyUpdated(Guid listId, UpdateBoardListRequest request)
    {
        var list = _boardLists.FirstOrDefault(bl => bl.Id == listId);
        if (list is null)
        {
            return;
        }
        list.Title = request.Title;
        list.Description = request.Description;
        Notify();
    }

    private void ApplyDeleted(Guid listId)
    {
        var list = _boardLists.FirstOrDefault(bl => bl.Id == listId);
        if (list is null)
        {
            return;
        }

        var deletedPosition = list.Position;
        _boardLists.Remove(list);

        foreach (var l in _boardLists.Where(bl => bl.Position > deletedPosition))
        {
            l.Position -= 1;
        }

        Notify();
    }

    private void Notify()
    {
        _sortedLists = null;
        OnChange?.Invoke();
    }
}