using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Services;
using Tracker.WebApp.Components.Boards;

namespace Tracker.WebApp.Pages.Workspaces;
public partial class Overview
{
    [Parameter]
    public Guid WorkspaceId { get; set; }

    private WorkspaceDto? Workspace { get; set; } = null;
    private readonly Random random = new Random();

    [Inject]
    private IWorkspaceService WorkspaceService { get; set; } = default!;
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

    private string RandomWidth()
    {
        int width = 30 + random.Next(0, 40);
        return $"{width}%";
    }

    private int RandomBoardsCount()
    {
        return 3 + random.Next(0, 3);
    }

    private static string GetBoardColorBackgroundStyle(BoardSummaryDto board)
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