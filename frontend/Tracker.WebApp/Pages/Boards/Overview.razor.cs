using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Pages.Boards;

public partial class Overview
{
    [Parameter]
    public Guid BoardId { get; set; }

    [Inject] private BoardState BoardState { get; set; } = null!;

    private BoardFullDto? Board => BoardState.CurrentBoard;

    protected override async Task OnInitializedAsync()
    {
        BoardState.OnChange += StateHasChanged;
        await BoardState.LoadBoardAsync(BoardId);
    }

    private string PageTitle() => Board?.Title ?? "Board";
}