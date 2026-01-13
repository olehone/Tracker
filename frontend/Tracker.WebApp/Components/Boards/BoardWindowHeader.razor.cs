using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardWindowHeader : IDisposable
{
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private BoardState BoardState { get; set; } = null!;

    private BoardFullDto? Board => BoardState.CurrentBoard;

    protected override void OnInitialized()
    {
        BoardState.OnChange += StateHasChanged;
    }

    private async Task OpenSettings()
    {
        if (Board == null)
        {
            return;
        }

        var settingsTitle = Board.Permissions.CanChangeBoard
            ? "Board settings"
            : "Board information";

        var dialog = await DialogService.ShowAsync<BoardSettingsDialog>(
            settingsTitle,
            new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true }
        );

        await dialog.Result;
    }

    private async Task OpenListsSwap()
    {
        if (Board == null)
        {
            return;
        }

        var dialog = await DialogService.ShowAsync<BoardListsSwapDialog>(
            $"Move lists of {Board.Title}",
            new DialogOptions { CloseButton = true }
        );

        await dialog.Result;
    }

    private async Task OpenMembers()
    {
        if (Board == null)
        {
            return;
        }

        var dialog = await DialogService.ShowAsync<BoardMembersDialog>(
            "Members",
            new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true }
        );

        await dialog.Result;
    }

    public void Dispose()
    {
        BoardState.OnChange -= StateHasChanged;
    }
}