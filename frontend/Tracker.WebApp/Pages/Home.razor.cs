using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Pages;

public partial class Home : IDisposable
{
    private bool _isAuthenticated;
    private List<BoardSummaryDto> Boards = [];

    [Inject] IBoardService BoardService { get; set; } = null!;
    [Inject] AppState AppState{ get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        AppState.OnUserChange +=  StateHasChangedHandler;
        await LoadBoardsIfAuthenticatedAsync();
    }

    private async Task LoadBoardsIfAuthenticatedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        _isAuthenticated = authState.User.Identity?.IsAuthenticated == true;

        if (!_isAuthenticated)
        {
            Boards = [];
            return;
        }

        var result = await BoardService.GetForCurrentUserAsync();
        if (result.IsFailure)
        {
            return;
        }

        Boards = result.Value;
    }

    private void OnAuthStateChanged(Task<AuthenticationState> task)
    {
        _ = InvokeAsync(async () =>
        {
            await LoadBoardsIfAuthenticatedAsync();
            StateHasChanged();
        });
    }


    private async Task StateHasChangedHandler()
    {
        await LoadBoardsIfAuthenticatedAsync();
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        BoardState.OnChange -= StateHasChangedHandler;
    }
}
