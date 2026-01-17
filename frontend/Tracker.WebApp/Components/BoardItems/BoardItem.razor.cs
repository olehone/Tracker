using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.Components.BoardLists;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardItems;

public partial class BoardItem
{
    [CascadingParameter]
    private BoardState BoardState { get; set; } = null!;

    [Parameter]
    public required BoardItemDto Item { get; set; }
    [Inject] IDialogService DialogService { get; set; } = null!;

    private async Task OpenItemSettings()
    {
        var parameters = new DialogParameters
        {
            { nameof(BoardItemSettingsDialog.BoardState), BoardState },
            { nameof(BoardItemSettingsDialog.Item), Item }
        };

        var dialog = await DialogService.ShowAsync<BoardItemSettingsDialog>(
            Item.Title,
            parameters
        );

        await dialog.Result;
    }
}