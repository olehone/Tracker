using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.Components.Boards;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardLists;

public partial class BoardList
{
    [CascadingParameter]
    private BoardState BoardState { get; set; } = null!;

    [Parameter]
    public required bool CanAddItem { get; set; }
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
            { nameof(BoardSettingsDialog.BoardState), BoardState }
        };

        var settingsTitle = Board.Permissions.CanChangeBoard
            ? "Board settings"
            : "Board information";

        var dialog = await DialogService.ShowAsync<BoardSettingsDialog>(
            settingsTitle,
            parameters,
            new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true }
        );

        await dialog.Result;
    }
}