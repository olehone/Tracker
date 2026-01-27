using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction.Auth;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Items;

public partial class ItemBrief
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

    private bool IsOwn()
    {
        if (BoardState.IsUnauthenticated)
        {
            return false;
        }
        return Item.Assignees.Contains(BoardState.CurrentUserId);
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
            { nameof(ItemSettingsDialog.BoardState), BoardState },
            { nameof(ItemSettingsDialog.Item), Item }
        };

        var dialog = await DialogService.ShowAsync<ItemSettingsDialog>(
            Item.Title,
            parameters,
            DialogOptions);

        await dialog.Result;
    }
}