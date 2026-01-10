using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Services;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardWindowHeader
{
    [Parameter]
    public required BoardFullDto Board { get; set; }

    [Inject] IDialogService DialogService { get; set; } = null!;
    [Inject] IBoardService BoardService { get; set; } = null!;

    private async Task OpenSettings()
    {
        var parameters = new DialogParameters
        {
            { nameof(BoardSettingsDialog.Board), Board }
        };
        var settingsTitle = Board.Permissions.CanChangeBoard
            ? "Board settings"
            : "Board information";

        var dialog = await DialogService.ShowAsync<BoardSettingsDialog>(
            settingsTitle,
            parameters,
            new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true }
        );

        var result = await dialog.Result;
        if(result is null || result.Canceled)
        {
            return;
        }

        if (result.Data is true)
        {
            await ReloadBoard();
        }
    }

    private async Task ReloadBoard()
    {
        var result = await BoardService.GetBoardByIdAsync(Board.Id);
        if (result.IsSuccess)
        {
            Board = result.Value;
        }
    }

}