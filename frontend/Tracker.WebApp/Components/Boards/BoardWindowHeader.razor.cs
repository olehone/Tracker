using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.Components.BoardUsers;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardWindowHeader
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

    protected override void InsideDispose()
    {
        base.InsideDispose();
        BoardState.OnBoardNotFound -= ToWorkspace;
    }

    private string GetUsersKey()
    {
        return string.Join("-", BoardState.UsersState.RecentActiveUsers()
        .Select(u => u.User.Id));
    }

    private void ToWorkspace()
    {
        if (BoardState.IsLoading)
        {
            return;
        }
        if (BoardState is null)
        {
            Nav.NavigateTo("/");
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