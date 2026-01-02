using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Services;

namespace Tracker.WebApp.Pages.Workspaces;

public partial class Overview
{
    [Parameter]
    public Guid WorkspaceId { get; set; }

    private WorkspaceDto? Workspace { get; set; } = null;

    [Inject]
    private IWorkspaceService WorkspaceService { get; set; } = default!;

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