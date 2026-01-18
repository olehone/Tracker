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

    [Inject] IDialogService DialogService { get; set; } = null!;
    [Inject] NavigationManager Nav { get; set; } = null!;

    private BoardFullDto Board => BoardState.Board;
    private bool _disposed;

    protected override void OnInitialized()
    {
        BoardState.OnChange += OnBoardStateChanged;
        BoardState.Lists.OnChange += OnBoardStateChanged;
        BoardState.OnBoardNotFound += ToWorkspace;
    }

    private void OnBoardStateChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    private void ToWorkspace()
    {
        if (BoardState.IsLoading)
        {
            return;
        }
        var workspaceId = Board.WorkspaceId;
        Nav.NavigateTo($"workspaces/{workspaceId}/overview");
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
            parameters
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
            parameters
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
            new DialogOptions { MaxWidth = MaxWidth.Small }
        );

        await dialog.Result;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                BoardState.Items.OnChange -= OnBoardStateChanged;
                BoardState.Lists.OnChange -= OnBoardStateChanged;
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}