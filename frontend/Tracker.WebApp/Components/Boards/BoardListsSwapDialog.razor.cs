using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardListsSwapDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;
    [Parameter, EditorRequired]
    public BoardState BoardState { get; set; } = null!;

    private MudDropContainer<BoardListDto> _container = null!;
    private List<BoardListDto>? Lists => BoardState.Board?.BoardLists;

    protected override void OnInitialized()
    {
        BoardState.Lists.OnChange += StateHasChanged;
    }

    private async Task SwapList(MudItemDropInfo<BoardListDto> dropInfo)
    {
        if (dropInfo.Item is null)
        {
            return;
        }

        await BoardState.Lists.MoveBoardListAsync(
            dropInfo.Item.Id,
            dropInfo.IndexInZone + 1
        );
    }
}