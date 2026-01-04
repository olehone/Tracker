using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Services.Abstraction.Entities;
using Tracker.WebApp.Shared;

namespace Tracker.WebApp.Pages.Workspaces;

public partial class Overview
{
    [Parameter]
    public Guid WorkspaceId { get; set; }

    [Inject] private IWorkspaceService WorkspaceService { get; set; } = null!;
    [Inject] private IBoardService BoardService { get; set; } = null!;

    private WorkspaceDto? Workspace { get; set; } = null;

    protected override async Task OnInitializedAsync()
    {
        var result = await WorkspaceService.GetWorkspaceByIdAsync(WorkspaceId);
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
            var result = await WorkspaceService.GetWorkspaceByIdAsync(WorkspaceId);
            if (result.IsFailure)
            {
                return;
            }
            Workspace = result.Value;
        }
    }

    private async Task CreateBoard(string title)
    {
        var request = new CreateBoardRequest()
        {
            WorkspaceId = WorkspaceId,
            Title = title
        };
        var result = await BoardService.CreateBoardAsync(request);
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
}