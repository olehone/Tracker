using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Tracker.Domain.Dtos;
using Tracker.Services;

namespace Tracker.WebApp.Components;
public partial class WorkspacesList
{

    private bool isAuthenticated;
    private List<WorkspaceDto>? Workspaces = null;
    private readonly Random random = new Random();

    [Inject]
    private IWorkspaceService WorkspaceService { get; set; } = default!;
    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    private string RandomWidth()
    {
        int width = 30 + random.Next(0, 40);
        return $"{width}%";
    }

    protected override async Task OnInitializedAsync()
    {
        AuthStateProvider.AuthenticationStateChanged += OnAuthStateChanged;
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        isAuthenticated = user.Identity?.IsAuthenticated == true;

        if (isAuthenticated && Workspaces is null)
        {
            Workspaces = await WorkspaceService.GetWorkspacesAsync();
        }
    }

    private async void OnAuthStateChanged(Task<AuthenticationState> task)
    {
        var authState = await task;
        if (authState.User.Identity?.IsAuthenticated == true)
        {
            Workspaces = await WorkspaceService.GetWorkspacesAsync();
            StateHasChanged();
        }
        else
        {
            Workspaces = null;
            StateHasChanged();
        }
    }
}