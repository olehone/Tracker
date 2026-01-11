using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardList;
using Tracker.Services.Abstraction;
using static MudBlazor.CategoryTypes;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardListsSwapDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }

    private MudDropContainer<BoardListDto> _container = null!;
    private List<BoardListDto> _lists = null!;
    [Parameter]
    public required BoardFullDto Board { get; set; }

    [Inject] private IBoardListService BoardListService { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        _lists = Board.BoardLists;

        StateHasChanged();
    }

    private async Task SwapList(MudItemDropInfo<BoardListDto> list)
    {
        if (list.Item is null)
        {
            return;
        }
        var request = new MoveBoardListRequest
        {
            BoardListId = list.Item.Id,
            Position = list.IndexInZone + 1,
        };
        await BoardListService.MoveBoardListAsync(request);
        _lists.UpdateOrder(list, item => item.Position);
        Board.BoardLists = Board.BoardLists.OrderBy(bl => bl.Position).ToList();
        StateHasChanged();
    }

    private void Submit()
    {
        MudDialog.Close(DialogResult.Ok(true));
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }

}