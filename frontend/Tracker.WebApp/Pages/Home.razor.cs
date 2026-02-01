using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.Pages;
public partial class Home : IAsyncDisposable
{
    private bool _isAuthenticated;
    private List<BoardSummaryDto> Boards = [];

    [Inject] private IBoardService BoardService { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        AuthStateProvider.AuthenticationStateChanged += OnAuthStateChanged;
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


    public ValueTask DisposeAsync()
    {
        AuthStateProvider.AuthenticationStateChanged -= OnAuthStateChanged;
        return ValueTask.CompletedTask;
    }
}
