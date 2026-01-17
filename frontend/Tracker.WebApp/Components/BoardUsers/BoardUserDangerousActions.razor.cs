using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Components.BoardUsers;

public partial class BoardUserDangerousActions
{
    [Parameter, EditorRequired]
    public BoardUserDto BoardUser { get; set; }

    [Inject] private IDialogService DialogService { get; set; } = null!;

    private async Task ShowTransferOwnershipDialog()
    {
        var parameters = new DialogParameters
        {
            { "NewOwnerName", BoardUser.User.Username }
        };

        var dialog = await DialogService.ShowAsync<TransferOwnershipDialog>(
            "Transfer Ownership",
            parameters,
            new DialogOptions { MaxWidth = MaxWidth.Small });

        var result = await dialog.Result;
        if (result is null || result.Canceled)
        {
            return;
        }

        await Users.TransferOwnershipAsync(BoardUser);
    }
}