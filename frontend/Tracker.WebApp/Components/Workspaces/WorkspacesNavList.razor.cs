using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Workspace;
using Tracker.Services;

namespace Tracker.WebApp.Components.Workspaces;

public partial class WorkspacesNavList : IAsyncDisposable
{
    private bool isAuthenticated;
    private List<WorkspaceDto>? Workspaces = null;

    [Inject]
    private IWorkspaceService WorkspaceService { get; set; } = default!;
    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

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
        var workspace = await WorkspaceService.CreateWorkspaceAsync(request);
        Workspaces!.Add(workspace);
        StateHasChanged();
    }

    private async Task LoadWorkspacesIfAuthenticatedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        isAuthenticated = authState.User.Identity?.IsAuthenticated == true;

        Workspaces = isAuthenticated
            ? await WorkspaceService.GetWorkspacesAsync()
            : null;
    }
    private async void OnAuthStateChanged(Task<AuthenticationState> task)
    {
        var authState = await task;
        var isAuth = authState.User.Identity?.IsAuthenticated == true;
        if (isAuth)
        {
            Workspaces = await WorkspaceService.GetWorkspacesAsync();
        }
        else
        {
            Workspaces = null;
        }

        await InvokeAsync(StateHasChanged);
    }

    public async ValueTask DisposeAsync()
    {
        AuthStateProvider.AuthenticationStateChanged -= OnAuthStateChanged;
        await Task.CompletedTask;
    }
}