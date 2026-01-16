using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardLists;

public partial class BoardList
{
    [CascadingParameter]
    private BoardState BoardState { get; set; } = null!;

    [Parameter]
    public required BoardListDto List { get; set; }

    [Parameter]
    public EventCallback<string> OnCreateItem { get; set; }
    [Inject] IDialogService DialogService { get; set; } = null!;

    private BoardFullDto Board => BoardState.Board!;

    private async Task CreateNewItem(string title)
    {
        await OnCreateItem.InvokeAsync(title);
    }

    private async Task OpenListSettings()
    {
        var parameters = new DialogParameters
        {
            { nameof(BoardListSettingsDialog.BoardState), BoardState },
            { nameof(BoardListSettingsDialog.List), List }
        };

        var dialog = await DialogService.ShowAsync<BoardListSettingsDialog>(
            List.Title,
            parameters,
            new DialogOptions { MaxWidth = MaxWidth.Medium }
        );

        await dialog.Result;
    }
}