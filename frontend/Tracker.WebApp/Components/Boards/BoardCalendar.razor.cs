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
    private bool _onlySelf = false;

    private List<CalendarItem> Items = [];

    private bool IsUnauthorized()
    {
        return AppState.CurrentUser is null;
    }

    protected override void OnInitialized()
    {
        BoardState.ItemsState.OnChange += OnBoardStateChanged;
        Items = BoardState.ItemsState.BoardItems
            .Where(bi => bi.DueDate.HasValue)
            .Where(IsOwn)
            .Select(bi => new BoardCalendarItemModel(bi))
            .Cast<CalendarItem>()
            .ToList();
    }

    private bool IsOwn(BoardItemDto item)
    {
        if (!_onlySelf)
        {
            return true;
        }
        if (AppState.CurrentUser is null)
        {
            return false;
        }
        return item.Assignees.Contains(AppState.CurrentUser.Id);
    }


    private void OnBoardStateChanged()
    {
        InvokeAsync(() =>
        {
            StateHasChanged();
        });
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