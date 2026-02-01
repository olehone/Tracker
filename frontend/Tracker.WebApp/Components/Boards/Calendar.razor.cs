using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;
using Tracker.WebApp.Components.Items;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class Calendar : IDisposable
{
    [CascadingParameter]
    private BoardState BoardState { get; set; } = null!;

    private bool _showNotOwn = true;
    private bool _showNotCompleted = true;

    private List<CalendarItemWrapper> ItemsWithDate = [];
    private List<BoardItemDto> ItemsWithoutDate = [];

    protected override void OnInitialized()
    {
        BoardState.ItemsState.OnChange += StateHasChangedHandler;
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

    private async Task OnItemChanged(CalendarItemWrapper item)
    {
        var localOffset = TimeZoneInfo.Local.GetUtcOffset(item.Start);
        var dueDate = new DateTimeOffset(
            item.Start.Year,
            item.Start.Month,
            item.Start.Day,
            23, 59, 59,
            localOffset);

        var request = new UpdateBoardItemRequest
        {
            DueDate = dueDate
        };
        await BoardState.ItemsState.UpdateAsync(item.Item.Id, request);
    }

    private void ReloadItems()
    {
        var items = BoardState.ItemsState.BoardItems
            .Where(OwnFilter)
            .Where(CompletedFilter);

        ItemsWithDate = items
            .Where(bi => bi.DueDate.HasValue)
            .Select(bi => new CalendarItemWrapper(bi))
            .ToList();

        ItemsWithoutDate = items
            .Where(bi => !bi.DueDate.HasValue)
            .ToList();
    }

    private bool OwnFilter(BoardItemDto item)
    {
        if (_showNotOwn)
        {
            return true;
        }
        if (BoardState.IsUnauthenticated)
        {
            return false;
        }
        return item.Assignees.Contains(BoardState.CurrentUserId);
    }

    private bool CompletedFilter(BoardItemDto item)
    {
        if (_showNotCompleted)
        {
            return true;
        }
        return !item.IsDone;
    }

    private void StateHasChangedHandler()
    {
        ReloadItems();
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        BoardState.ItemsState.OnChange -= StateHasChangedHandler;
    }
}