using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardLists
{
    private MudDropContainer<BoardItemDto> _container = null!;

    [Parameter]
    public Guid BoardId { get; set; }

    [Inject] private BoardState BoardState { get; set; } = null!;

    private BoardFullDto Board => BoardState.CurrentBoard!;

    protected override void OnInitialized()
    {
        BoardState.OnChange += StateHasChanged;
    }
    private List<BoardItemDto> Items()
    {
        return Board?.BoardLists.SelectMany(bl => bl.BoardItems).ToList() ?? [];
    }

    private void ItemDropped(MudItemDropInfo<BoardItemDto> dropInfo)
    {
        if (dropInfo.Item is null)
        {
            return;
        }

        _ = BoardState.MoveBoardItemAsync(
            dropInfo.Item.Id.ToString(),
            dropInfo.DropzoneIdentifier,
            dropInfo.IndexInZone + 1
        );
        _container.Refresh();
    }

    private async Task CreateList(string title)
    {
        await BoardState.CreateBoardListAsync(title);
    }

    private async Task CreateItemForList(BoardListDto list, string title)
    {
        await BoardState.CreateBoardItemAsync(list.Id, title);
        _container.Refresh();
    }
}