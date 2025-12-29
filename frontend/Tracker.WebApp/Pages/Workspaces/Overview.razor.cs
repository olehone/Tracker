using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Services;

namespace Tracker.WebApp.Pages.Workspaces;
public partial class Overview
{
    [Parameter]
    public Guid WorkspaceId { get; set; }

    private WorkspaceDto? Workspace { get; set; } = null;
    private bool _showDialog = false;
    private readonly Random random = new Random();

    [Inject]
    private IWorkspaceService WorkspaceService { get; set; } = default!;

    [Inject]
    private NavigationManager Nav { get; set; } = default!;

    private void OpenDialog()
    {
        _showDialog = true;
    }

    private void HandleBoardCreated(BoardSummaryDto board)
    {
        Workspace?.Boards.Add(board);
        StateHasChanged();
    }

    private string RandomWidth()
    {
        int width = 30 + random.Next(0, 40);
        return $"{width}%";
    }

    private int RandomBoardsCount()
    {
        return 3 + random.Next(0, 3);
    }

    private string GetBoardColorBackgroundStyle(BoardSummaryDto board)
    {
        var hash = board.Id.GetHashCode();
        var hue = Math.Abs(hash % 360);
        return $"background:hsl({hue}, 60%, 55%);";
    }

    protected override async Task OnInitializedAsync()
    {
        Workspace = await WorkspaceService.GetWorkspaceByIdAsync(WorkspaceId);
        StateHasChanged();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Workspace == null || Workspace.Id != WorkspaceId)
        {
            Workspace = null;
            StateHasChanged();
            Workspace = await WorkspaceService.GetWorkspaceByIdAsync(WorkspaceId);
        }
    }
}