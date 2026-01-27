using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardItems;

public partial class BoardItemBrief
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
    public BoardItemDto Item { get; set; } = null!;
    [Parameter]
    public Size Size { get; set; } = Size.Large;
    [Parameter]
    public bool OnlyTitle { get; set; } = false;

    [Inject] IDialogService DialogService { get; set; } = null!;
    [Inject] AppState AppState { get; set; } = null!;

    private bool IsOwn()
    {
        if (AppState.CurrentUser is null)
        {
            return false;
        }
        return Item.Assignees.Contains(AppState.CurrentUser.Id);
    }

    private string GetStyle()
    {
        return Item.IsDone
            ? "text-decoration: line-through; width: 90%"
            : "width: 90%";
    }
    private Color GetColor()
    {
        if (Item.IsDone)
        {
            return Color.Success;
        }
        return ImportanceHelper.GetColor(Item.Importance);
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
            { nameof(BoardItemSettingsDialog.Item), Item }
        };

        var dialog = await DialogService.ShowAsync<BoardItemSettingsDialog>(
            Item.Title,
            parameters,
            DialogOptions);

        await dialog.Result;
    }
}