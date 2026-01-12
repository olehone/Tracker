using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardListsSwapDialog
{
    private MudDropContainer<BoardListDto> _container = null!;
    
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;

    [Inject] private BoardState BoardState { get; set; } = null!;

    private List<BoardListDto>? Lists => BoardState.CurrentBoard?.BoardLists;

    protected override void OnInitialized()
    {
        BoardState.OnChange += StateHasChanged;
    }

    private async Task SwapList(MudItemDropInfo<BoardListDto> dropInfo)
    {
        if (dropInfo.Item is null)
        {
            return;
        }

        await BoardState.MoveBoardListAsync(
            dropInfo.Item.Id,
            dropInfo.IndexInZone + 1
        );
    }

    private void Cancel() => MudDialog.Close(DialogResult.Ok(true));
}