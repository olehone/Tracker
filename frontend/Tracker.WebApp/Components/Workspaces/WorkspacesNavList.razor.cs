using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Workspace;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.Components.Workspaces;

public partial class WorkspacesNavList : IAsyncDisposable
{
    private bool _isAuthenticated;
    private List<WorkspaceSummaryDto>? Workspaces;
    [Inject] private IWorkspaceService WorkspaceService { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;

    public async ValueTask DisposeAsync()
    {
        AuthStateProvider.AuthenticationStateChanged -= StateHasChangedHandler;
        await Task.CompletedTask;
    }

    protected override async Task OnInitializedAsync()
    {
        AuthStateProvider.AuthenticationStateChanged += StateHasChangedHandler;
        await LoadWorkspacesIfAuthenticatedAsync();
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

        Workspaces!.Add(result.Value);
        StateHasChanged();
    }

    private async Task LoadWorkspacesIfAuthenticatedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        _isAuthenticated = authState.User.Identity?.IsAuthenticated == true;
        if (!_isAuthenticated)
        {
            Workspaces = null;
            return;
        }

        var result = await WorkspaceService.GetForCurrentUserAsync();
        if (result.IsFailure)
        {
            return;
        }

        Workspaces = result.Value;
    }

    private async void StateHasChangedHandler(Task<AuthenticationState> task)
    {
        await LoadWorkspacesIfAuthenticatedAsync();
        await InvokeAsync(StateHasChanged);
    }
}