using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Domain.Requests.BoardItem;
using Tracker.Domain.Requests.Workspace;
using Tracker.Services;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.Pages.Workspaces;

public partial class Overview
{
    [Parameter]
    public Guid WorkspaceId { get; set; }

    private WorkspaceDto? Workspace { get; set; } = null;

    [Inject]
    private IWorkspaceService WorkspaceService { get; set; } = default!;
    [Inject]
    private IBoardService BoardService { get; set; } = default!;

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

    private async Task CreateBoard(string title)
    {
        var request = new CreateBoardRequest()
        {
            WorkspaceId = WorkspaceId,
            Title = title
        };
        var board = await BoardService.CreateBoardAsync(request);
        Workspace!.Boards.Add(board);
        StateHasChanged();
    }
}