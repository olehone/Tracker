using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction.Board;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Pages;

public partial class Home : IDisposable
{
    private List<BoardSummaryDto> Boards = [];

    [Inject] private IBoardService BoardService { get; set; } = null!;
    [Inject] private AppState AppState { get; set; } = null!;

    protected override void OnInitialized()
    {
        AppState.OnChange += HandleUserChanged;

        if (AppState.IsAuthenticated)
        {
            TriggerBoardsReloadAsync();
        }
    }

    private Task HandleUserChanged()
    {
        return TriggerBoardsReloadAsync();
    }

    private Task TriggerBoardsReloadAsync()
    {
        return InvokeAsync(async () =>
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
        AppState.OnChange -= HandleUserChanged;
    }
}