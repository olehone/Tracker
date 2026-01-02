using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Components.Boards;

public partial class CreateBoardButton
{
    [Parameter]
    public required WorkspaceDto Workspace { get; set; }

    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    private async Task OpenDialog()
    {
        var parameters = new DialogParameters
        {
            { "Workspace", Workspace }
        };

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        var dialog = await DialogService.ShowAsync<CreateBoardDialog>("Create New Board", parameters, options);
        var result = await dialog.Result;

        if (result is not null && !result.Canceled && result.Data is BoardSummaryDto board)
        {
            HandleBoardCreated(board);
        }
    }

    private void HandleBoardCreated(BoardSummaryDto board)
    {
        Workspace?.Boards.Add(board);
        StateHasChanged();
    }
}