using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardItems;

public partial class BoardCalendarItem
{
    private static readonly DialogOptions DialogOptions = new()
    {
        CloseButton = false,
        NoHeader = true,
        MaxWidth = MaxWidth.Small
    };

    [CascadingParameter]
    private BoardState BoardState { get; set; } = null!;

    [Parameter, EditorRequired]
    public BoardCalendarItemModel Item { get; set; } = null!;

    [Inject] IDialogService DialogService { get; set; } = null!;
    [Inject] AppState AppState { get; set; } = null!;

    private BoardItemDto BoardItem => Item.BoardItem;

    private string ItemStyle =>
        BoardItem.IsDone ? "text-decoration: line-through; width: 100%" : "width: 100%";

    private bool IsOwn()
    {
        if (AppState.CurrentUser is null)
        {
            return false;
        }
        return BoardItem.Assignees.Contains(AppState.CurrentUser.Id);
    }

    private Color GetColor()
    {
        if (BoardItem.IsDone)
        {
            return Color.Success;
        }
        return ImportanceHelper.GetColor(BoardItem.Importance);
    }

    private Variant GetVariant()
    {
        return IsOwn()
            ? Variant.Filled
            : Variant.Outlined;
    }

    private async Task OpenItemSettings()
    {
        var parameters = new DialogParameters
        {
            { nameof(BoardItemSettingsDialog.BoardState), BoardState },
            { nameof(BoardItemSettingsDialog.Item), BoardItem }
        };

        var dialog = await DialogService.ShowAsync<BoardItemSettingsDialog>(
            BoardItem.Title,
            parameters,
            DialogOptions);

        await dialog.Result;
    }
}