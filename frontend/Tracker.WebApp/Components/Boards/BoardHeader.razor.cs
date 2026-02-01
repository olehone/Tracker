using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.WebApp.Components.BoardUsers;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardHeader
{
    [Inject] IDialogService DialogService { get; set; } = null!;
    [Inject] NavigationManager Nav { get; set; } = null!;

    [Parameter, EditorRequired]
    public int TabIndex { get; set; }

    [Parameter]
    public EventCallback<int> TabIndexChanged { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        BoardState.OnBoardNotFound += ToWorkspace;
    }

    public override void Dispose()
    {
        base.Dispose();
        BoardState.OnBoardNotFound -= ToWorkspace;
    }

    private void ToWorkspace()
    {
        Nav.NavigateTo(WorkspacePath());
    }

    private string WorkspacePath()
    {
        if (BoardState is null || BoardState.IsLoading)
        {
            return "/";
        }

        var workspaceId = Board.WorkspaceId;
        return $"workspaces/{workspaceId}/overview";
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
            { nameof(ListsSwapDialog.BoardState), BoardState }
        };
        var dialog = await DialogService.ShowAsync<ListsSwapDialog>(
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

    private void OnTabIndexChanged(int index)
    {
        TabIndex = index;
        TabIndexChanged.InvokeAsync(index);
    }
}