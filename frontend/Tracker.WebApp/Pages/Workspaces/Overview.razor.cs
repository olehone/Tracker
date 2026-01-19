using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.Pages.Workspaces;

public partial class Overview
{
    [Parameter]
    public Guid WorkspaceId { get; set; }

    [Inject] private IWorkspaceService WorkspaceService { get; set; } = null!;
    [Inject] private IBoardService BoardService { get; set; } = null!;

    private WorkspaceFullDto? Workspace { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var result = await WorkspaceService.GetByIdAsync(WorkspaceId);
        if (result.IsFailure)
        {
            return;
        }

        Workspace = result.Value;
        StateHasChanged();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Workspace == null || Workspace.Id != WorkspaceId)
        {
            Workspace = null;
            StateHasChanged();
            var result = await WorkspaceService.GetByIdAsync(WorkspaceId);
            if (result.IsFailure)
            {
                return;
            }

            Workspace = result.Value;
        }
    }

    private async Task CreateBoard(string title)
    {
        var request = new CreateBoardRequest
        {
            WorkspaceId = WorkspaceId,
            Title = title
        };
        var result = await BoardService.CreateAsync(request);
        if (result.IsFailure)
        {
            return;
        }

        Workspace!.Boards.Add(result.Value);
        StateHasChanged();
    }

    private string PageTitle()
    {
        return Workspace?.Title ?? "Workspace";
    }

    private bool CanCreateBoard()
    {
        return Workspace?.Permissions.CanCreateBoard ?? false;
    }

}