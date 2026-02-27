using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Services.Abstraction.Board;
using Tracker.WebApp.Shared;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardSummary
{
    [Parameter, EditorRequired]
    public BoardSummaryDto Board { get; set; }
    [Parameter]
    public bool IsInWorkspace { get; set; }

    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private IBoardService BoardService { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

    private string? _customColor;
    private bool IsArchived => Board.IsArchived;

    private async Task OpenBoard()
    {
        if (IsArchived)
        {
            if (IsInWorkspace)
            {
                await ShowArchiveMessage();
            }
            else
            {
                Nav.NavigateTo($"/workspaces/{Board.WorkspaceId}/overview");
            }
        }
        else
        {
            Nav.NavigateTo($"/boards/{Board.Id}");
        }
    }

    private async Task ShowArchiveMessage()
    {
        if (!Board.IsAbleToUnarchive)
        {
            await DialogService.ShowMessageBox(
                "Archived",
                "Someone archived this board",
                cancelText: "Cancel");
            return;
        }

        if (Board.ArchiveStatus != ArchiveStatus.Archived)
        {
            await DialogService.ShowMessageBox(
                ArchiveStatusHelper.GetDescription(Board.ArchiveStatus),
                "Board in archiving process, try again later",
                cancelText: "Cancel");
            return;
        }

        var dialogResult = await DialogService.ShowMessageBox(
            "Warning",
            "Do you want to move this board from archive? This could take some time",
            yesText: "Unarchive", cancelText: "Cancel");
        if (dialogResult == null)
        {
            return;
        }

        var result = await BoardService.UnarchiveAsync(Board.Id);
        if (result.IsSuccess)
        {
            Board.ArchiveStatus = ArchiveStatus.PendingUnarchive;
        }
    }

    private string CustomColor
    {
        get
        {
            _customColor ??= UiHelper.GetColorById(Board.Id);
            return _customColor;
        }
    }
}