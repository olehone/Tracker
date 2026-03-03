using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction.Workspace;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Workspaces;

public partial class WorkspacesNavList : IDisposable
{
    private List<WorkspaceSummaryDto>? _workspaces;

    [Inject] private AppState AppState { get; set; } = null!;
    [Inject] private IWorkspaceService WorkspaceService { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        AppState.OnChange += StateHasChangedHandler;
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

    private Task StateHasChangedHandler()
    {
        return InvokeAsync(async () =>
        {
            await LoadWorkspaces();
            StateHasChanged();
        });
    }

    private async Task LoadWorkspaces()
    {
        if (AppState.IsUnauthenticated)
        {
            return;
        }

        var result = await WorkspaceService.GetForCurrentUserAsync();
        if (result.IsFailure)
        {
            return;
        }

        _workspaces = result.Value;
    }

    public void Dispose()
    {
        AppState.OnChange -= StateHasChangedHandler;
    }
}