using Microsoft.AspNetCore.Components;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Realtime;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Pages.Boards;

public partial class Overview : IDisposable
{
    [Parameter]
    public Guid BoardId { get; set; }
    [Parameter, SupplyParameterFromQuery(Name = "item")]
    public Guid? ItemId { get; set; }

    [Inject] AppState AppState { get; set; } = null!;
    [Inject] IBoardRealtimeService BoardRealtime { get; set; } = null!;
    [Inject] IBoardService BoardService { get; set; } = null!;
    [Inject] IBoardListService BoardListService { get; set; } = null!;
    [Inject] IBoardItemService BoardItemService { get; set; } = null!;
    [Inject] IBoardUserService BoardUserService { get; set; } = null!;
    [Inject] IUserService UserService { get; set; } = null!;

    private BoardState BoardState { get; set; } = null!;
    private int activeIndex = 0;

    protected override async Task OnInitializedAsync()
    {
        BoardState = new BoardState(
            AppState,
            BoardService,
            BoardListService,
            BoardItemService,
            BoardUserService,
            UserService,
            BoardRealtime);
        await BoardState.LoadAsync(BoardId);

        if (ItemId.HasValue)
        {
            activeIndex = 0;
        }

        BoardState.OnChange += StateHasChangedHandler;
    }

    private string PageTitle() => BoardState.IsLoading
        ? "Board loading"
        : BoardState.Board.Title;

    private void StateHasChangedHandler()
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        BoardState.OnChange -= StateHasChangedHandler;
    }
}