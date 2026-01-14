using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardUsers;

public partial class BoardUserDangerousActions
{
    [CascadingParameter]
    private BoardState BoardState { get; set; } = null!;

    [Parameter, EditorRequired]
    public BoardUserDto BoardUser { get; set; }

    [Inject] private IDialogService DialogService { get; set; } = null!;

    private BoardUsersState Users => BoardState.Users;

    protected override void OnInitialized()
    {
        BoardState.OnChange += StateHasChanged;
    }

    private async Task ShowTransferOwnershipDialog()
    {
        var parameters = new DialogParameters
    {
        { "NewOwnerName", BoardUser.User.Username }
    };

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        var dialog = await DialogService.ShowAsync<TransferOwnershipDialog>(
            "Transfer Ownership",
            parameters,
            options);

        var result = await dialog.Result;
        if (result is null || result.Canceled)
        {
            return;
        }

        await Users.TransferOwnershipAsync(BoardUser);
    }
}