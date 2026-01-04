using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Workspace;
using Tracker.Services.Abstraction.Entities;
using Tracker.WebApp.Shared;

namespace Tracker.WebApp.Components.Workspaces;

public partial class WorkspacesNavList : IAsyncDisposable
{
    [Inject] private IWorkspaceService WorkspaceService { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;

    private bool isAuthenticated;
    private List<WorkspaceDto>? Workspaces = null;

    protected override async Task OnInitializedAsync()
    {
        AuthStateProvider.AuthenticationStateChanged += OnAuthStateChanged;
        await LoadWorkspacesIfAuthenticatedAsync();
    }

    private async Task CreateWorkspace(string title)
    {
        var request = new CreateWorkspaceRequest()
        {
            Title = title
        };
        var result = await WorkspaceService.CreateWorkspaceAsync(request);
        if (result.IsFailure)
        {
            return;
        }
        Workspaces!.Add(result.Value);
        StateHasChanged();
    }

    private async Task LoadWorkspacesIfAuthenticatedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        isAuthenticated = authState.User.Identity?.IsAuthenticated == true;
        if (!isAuthenticated)
        {
            Workspaces = null;
        }

        var result = await WorkspaceService.GetWorkspacesAsync();
        if (result.IsFailure)
        {
            return;
        }
        Workspaces = result.Value;
    }

    private async void OnAuthStateChanged(Task<AuthenticationState> task)
    {
        await LoadWorkspacesIfAuthenticatedAsync();
        await InvokeAsync(StateHasChanged);
    }

    public async ValueTask DisposeAsync()
    {
        AuthStateProvider.AuthenticationStateChanged -= OnAuthStateChanged;
        await Task.CompletedTask;
    }
}