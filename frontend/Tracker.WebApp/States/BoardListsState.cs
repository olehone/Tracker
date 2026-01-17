using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardList;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.States;

public sealed class BoardListsState(
    BoardState boardState,
    IBoardListService boardListService)
{
    private readonly List<BoardListDto> _boardLists = [];

    public IReadOnlyList<BoardListDto> BoardLists => _boardLists;
    public event Action? OnChange;

    private BoardFullDto Board => boardState.Board!;

    public void Reload()
    {
        var lists = Board.BoardLists;
        _boardLists.Clear();
        _boardLists.AddRange(lists);
    }

    public async Task CreateBoardListAsync(string title)
    {
        var request = new CreateBoardListRequest
        {
            Title = title
        };

        var result = await boardListService.CreateBoardListAsync(Board.Id, request);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
            return;
        }

        ApplyListCreated(result.Value);
    }

    public async Task MoveBoardListAsync(Guid listId, int newPosition)
    {
        var request = new MoveBoardListRequest
        {
            Position = newPosition
        };

        ApplyListMoved(listId, newPosition);

        var result = await boardListService.MoveBoardListAsync(listId, request);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
        }
    }

    public async Task UpdateBoardListAsync(Guid listId, UpdateBoardListRequest request)
    {
        ApplyListUpdated(listId, request);

        var result = await boardListService.UpdateBoardListAsync(listId, request);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
        }
    }

    public async Task DeleteBoardListAsync(Guid listId)
    {
        ApplyListDeleted(listId);

        var result = await boardListService.DeleteBoardListAsync(listId);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
        }
    }

    private void ApplyListCreated(BoardListDto newList)
    {
        _boardLists.Add(newList);
        _boardLists.Sort((a, b) => a.Position.CompareTo(b.Position));
        Notify();
    }

    private void ApplyListMoved(Guid listId, int newPosition)
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

    private void ApplyListUpdated(Guid listId, UpdateBoardListRequest request)
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

    private void ApplyListDeleted(Guid listId)
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

    private void Notify() => OnChange?.Invoke();
}