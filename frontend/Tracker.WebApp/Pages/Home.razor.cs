using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Pages;

public partial class Home : IDisposable
{
    private List<BoardSummaryDto> Boards = [];

    [Inject] IBoardService BoardService { get; set; } = null!;
    [Inject] AppState AppState { get; set; } = null!;

    protected override void OnInitialized()
    {
        AppState.OnUserChange += HandleUserChanged;

        if (AppState.IsAuthenticated)
        {
            TriggerBoardsReload();
        }
    }

    private void HandleUserChanged()
    {
        TriggerBoardsReload();
    }

    private void TriggerBoardsReload()
    {
        _ = InvokeAsync(async () =>
        {
            await LoadBoardsAsync();
            StateHasChanged();
        });
    }

    private async Task LoadBoardsAsync()
    {
        if (!AppState.IsAuthenticated)
        {
            Boards.Clear();
            return;
        }

        var result = await BoardService.GetForCurrentUserAsync();

        Boards = result.IsSuccess
            ? result.Value
            : [];
    }

    public void Dispose()
    {
        AppState.OnUserChange -= HandleUserChanged;
    }
}