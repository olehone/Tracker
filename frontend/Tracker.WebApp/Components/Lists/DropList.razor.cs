using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Lists;

public partial class DropList
{
    [CascadingParameter]
    private BoardState BoardState { get; set; } = null!;

    [Parameter]
    public required BoardListDto List { get; set; }

    [Inject] IDialogService DialogService { get; set; } = null!;

    private BoardFullDto Board => BoardState.Board!;

    private async Task CreateNewItem(string title)
    {
        await BoardState.ItemsState.CreateAsync(List.Id, title);
    }

    private async Task OpenListSettings()
    {
        var parameters = new DialogParameters
        {
            { nameof(ListSettingsDialog.BoardState), BoardState },
            { nameof(ListSettingsDialog.List), List }
        };

        var dialog = await DialogService.ShowAsync<ListSettingsDialog>(
            List.Title,
            parameters
        );

        await dialog.Result;
    }
}