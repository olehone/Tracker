using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.Components.BoardUsers;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardWindowHeader : IDisposable
{
    [CascadingParameter]
    private BoardState BoardState { get; set; } = null!;

    [Inject] private IDialogService DialogService { get; set; } = null!;

    private BoardFullDto Board => BoardState.Board;

    protected override void OnInitialized()
    {
        BoardState.OnChange += StateHasChanged;
    }

    private async Task OpenSettings()
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

    private async Task OpenListsSwap()
    {
        var parameters = new DialogParameters
        {
            { nameof(BoardListsSwapDialog.BoardState), BoardState }
        };
        var dialog = await DialogService.ShowAsync<BoardListsSwapDialog>(
            $"Move lists of {Board.Title}",
            parameters,
            new DialogOptions { CloseButton = true }
        );

        await dialog.Result;
    }

    private async Task OpenMembers()
    {
        var parameters = new DialogParameters
        {
            { nameof(BoardUsersDialog.BoardState), BoardState }
        };
        var dialog = await DialogService.ShowAsync<BoardUsersDialog>(
            "Members",
            parameters,
            new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, CloseButton = true }
        );

        await dialog.Result;
    }

    public void Dispose()
    {
        BoardState.OnChange -= StateHasChanged;
    }
}