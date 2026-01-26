using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;
using Tracker.WebApp.Components.BoardItems;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardCalendar : IDisposable
{
    [CascadingParameter]
    private BoardState BoardState { get; set; } = null!;

    [Inject] AppState AppState { get; set; } = null!;

    private bool _disposed;
    private bool _showNotOwn = true;
    private bool _showNotCompleted = true;

    private List<BoardCalendarItemModel> Items = [];
    private List<BoardItemDto> AllItems() => BoardState.ItemsState.BoardItems.ToList();

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

    private async Task OnItemChanged(BoardCalendarItemModel item)
    {
        var localOffset = TimeZoneInfo.Local.GetUtcOffset(item.Start);
        var dueDate = new DateTimeOffset(
            item.Start.Year,
            item.Start.Month,
            item.Start.Day,
            23,
            59,
            59,
            localOffset);
        var request = new UpdateBoardItemRequest
        {
            DueDate = dueDate
        };
        await BoardState.ItemsState.UpdateAsync(item.BoardItem.Id, request);
    }

    private async Task OnItemDroppedBetweenZones(MudItemDropInfo<BoardItemDto> dropInfo)
    {
        var item = dropInfo.Item;

        if (dropInfo.DropzoneIdentifier == "BoardItems")
        {
            var request = new UpdateBoardItemRequest { DueDate = null };
            await BoardState.ItemsState.UpdateAsync(item.Id, request);
        }
    }

    private bool GetItemsForDropzone(BoardItemDto item, string dropzone)
    {
        if (dropzone == "BoardItems")
        {
            return !item.DueDate.HasValue;
        }

        return item.DueDate.HasValue;
    }

    private void ReloadItems()
    {
        Items = BoardState.ItemsState.BoardItems
            .Where(bi => bi.DueDate.HasValue)
            .Where(OwnFilter)
            .Where(CompletedFilter)
            .Select(bi => new BoardCalendarItemModel(bi))
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
        ReloadItems();
        StateHasChanged();
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