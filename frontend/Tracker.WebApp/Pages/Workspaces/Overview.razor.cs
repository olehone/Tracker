using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction.Workspace;

namespace Tracker.WebApp.Pages.Workspaces;

public partial class Overview
{
    [Parameter]
    public Guid WorkspaceId { get; set; }

    [Inject] IWorkspaceService WorkspaceService { get; set; } = null!;
    [Inject] NavigationManager Nav { get; set; } = null!;

    private WorkspaceFullDto? Workspace { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var result = await WorkspaceService.GetByIdAsync(WorkspaceId);
        if (result.IsFailure)
        {
            return;
        }

        Workspace = result.Value;
        await InvokeAsync(StateHasChanged);
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Workspace == null || Workspace.Id != WorkspaceId)
        {
            Workspace = null;
            await InvokeAsync(StateHasChanged);
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
        if (Workspace is null || string.IsNullOrWhiteSpace(title))
        {
            return;
        }
        var result = await WorkspaceService.CreateBoardAsync(Workspace.Id, title);
        if (result.IsFailure)
        {
            return;
        }

        Workspace!.Boards.Add(result.Value);
        await InvokeAsync(StateHasChanged);
    }

    private string PageTitle()
    {
        return Workspace?.Title ?? "Workspace";
    }

    private bool CanCreateBoard()
    {
        return Workspace?.Permissions.CanCreateBoard ?? false;
    }

    private void ToSettings()
    {
        Nav.NavigateTo($"workspaces/{WorkspaceId}/settings");
    }

    private void ToUsers()
    {
        Nav.NavigateTo($"workspaces/{WorkspaceId}/users");
    }

}