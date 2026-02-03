using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Workspaces;

public partial class WorkspacesNavList : IDisposable
{
    private List<WorkspaceSummaryDto>? _workspaces;

    [Inject] AppState AppState { get; set; } = null!;
    [Inject] IWorkspaceService WorkspaceService { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        AppState.OnUserChange += StateHasChangedHandler;
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

    private void StateHasChangedHandler()
    {
        _ = InvokeAsync(async () =>
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
        AppState.OnUserChange -= StateHasChangedHandler;
    }
}