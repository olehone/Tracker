using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.Requests.BoardItem;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Items;

public partial class ItemSettingsDialog : IDisposable
{
    private bool _openAssign;
    private string _description = string.Empty;
    private DateTime? _date;
    private BoardItemImportance _importance;
    private bool _isEditingDescription = false;
    private bool _openDate = false;

    [Parameter]
    public BoardState BoardState { get; set; } = null!;

    [Parameter, EditorRequired]
    public BoardItemDto Item { get; set; } = null!;

    private bool IsItemExists =>
        BoardState.ItemsState.BoardItems.Any(i => i.Id == Item.Id);
    private bool Disabled =>
        !BoardState.Board.Permissions.CanChangeItem;

    private void ToggleAssign()
    {
        _openAssign = !_openAssign;
    }

    protected override void OnInitialized()
    {
        BoardState.ItemsState.OnChange += StateHasChangedHandler;
        _description = Item.Description;
        _date = Item.DueDate?.UtcDateTime;
        _importance = Item.Importance;
    }

    private void StateHasChangedHandler()
    {
        if (!_isEditingDescription && _description != Item.Description)
        {
            _description = Item.Description;
        }

        _date = Item.DueDate?.UtcDateTime;
        _importance = Item.Importance;
        InvokeAsync(StateHasChanged);
    }

    private void DescriptionFocused()
    {
        _isEditingDescription = true;
    }

    private async Task DescriptionBlurred()
    {
        _isEditingDescription = false;

        if (_description == Item.Description)
        {
            return;
        }

        var request = new UpdateBoardItemRequest { Description = _description };
        await BoardState.ItemsState.UpdateAsync(Item.Id, request);
    }

    private async Task RemoveDueDate()
    {
        var request = new UpdateBoardItemRequest
        {
            ClearDueDate = true
        };
        _openDate = false;

        await BoardState.ItemsState.UpdateAsync(Item.Id, request);
    }

    private async Task DateSelected(DateTime? date)
    {
        if (date is null)
        {
            return;
        }
        if (_date == date)
        {
            return;
        }
        _date = date;
        var localOffset = TimeZoneInfo.Local.GetUtcOffset(date.Value);
        var dueDate = new DateTimeOffset(
            date.Value.Year,
            date.Value.Month,
            date.Value.Day,
            23,
            59,
            59,
            localOffset);
        var request = new UpdateBoardItemRequest
        {
            DueDate = dueDate
        };

        await BoardState.ItemsState.UpdateAsync(Item.Id, request);
    }

    private async Task ImportanceSelected(BoardItemImportance importance)
    {
        if (_importance == importance)
        {
            return;
        }
        _importance = importance;
        var request = new UpdateBoardItemRequest
        {
            Importance = importance
        };

        await BoardState.ItemsState.UpdateAsync(Item.Id, request);
    }

    public void Dispose()
    {
        BoardState.ItemsState.OnChange -= StateHasChangedHandler;
    }
}