using Heron.MudCalendar;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.Components.BoardItems;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardCalendar : IDisposable
{
    [CascadingParameter]
    private BoardState BoardState { get; set; } = null!;

    [Inject] AppState AppState { get; set; } = null!;

    private bool _disposed;
    private bool _showNotOwn = false;
    private bool _showNotCompleted = false;

    private List<CalendarItem> Items = [];

    private bool IsUnauthorized()
    {
        return AppState.CurrentUser is null;
    }

    protected override void OnInitialized()
    {
        BoardState.ItemsState.OnChange += OnBoardStateChanged;
        ReloadItems();
    }

    private void OnNotOwnChanged()
    {
        _showNotOwn = !_showNotOwn;
        ReloadItems();
    }

    private void OnNotCompletedChanged()
    {
        _showNotCompleted = !_showNotCompleted;
        ReloadItems();
    }

    private void ReloadItems()
    {
        Items = BoardState.ItemsState.BoardItems
            .Where(bi => bi.DueDate.HasValue)
            .Where(OwnFilter)
            .Where(CompletedFilter)
            .Select(bi => new BoardCalendarItemModel(bi))
            .Cast<CalendarItem>()
            .ToList();
    }

    private bool OwnFilter(BoardItemDto item)
    {
        if (_showNotOwn)
        {
            return true;
        }
        if (AppState.CurrentUser is null)
        {
            return false;
        }
        return item.Assignees.Contains(AppState.CurrentUser.Id);
    }

    private bool CompletedFilter(BoardItemDto item)
    {
        if (_showNotCompleted)
        {
            return true;
        }
        return !item.IsDone;
    }

    private void OnBoardStateChanged()
    {
        StateHasChanged();
        ReloadItems();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                BoardState.ItemsState.OnChange -= OnBoardStateChanged;
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}