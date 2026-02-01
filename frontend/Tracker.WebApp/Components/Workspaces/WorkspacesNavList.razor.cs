using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.Components.Workspaces;

public partial class WorkspacesNavList
{
    private List<WorkspaceSummaryDto>? _workspaces;

    [Inject] IWorkspaceService WorkspaceService { get; set; } = null!;
    protected override async Task OnInitializedAsync()
    {
        await LoadWorkspaces();
    }

    private async Task CreateWorkspace(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return;
        }
        var result = await WorkspaceService.CreateAsync(title);
        if (result.IsFailure)
        {
            return;
        }

        _workspaces!.Add(result.Value);
        StateHasChanged();
    }

    private async Task LoadWorkspaces()
    {
        var result = await WorkspaceService.GetForCurrentUserAsync();
        if (result.IsFailure)
        {
            return;
        }

        _workspaces = result.Value;
    }
}