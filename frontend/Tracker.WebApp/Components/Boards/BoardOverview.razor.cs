using Microsoft.AspNetCore.Components;
using MudBlazor;
using Polly.Simmy.Behavior;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardOverview
{
    [CascadingParameter] 
    private BoardState BoardState { get; set; } = null!;
    private BoardFullDto Board => BoardState.Board!;
    private MudDropContainer<BoardItemDto> _container = null!;

    protected override void OnInitialized()
    {
        BoardState.OnChange += OnBoardStateChanged;
    }

    private void OnBoardStateChanged()
    {
        StateHasChanged();
        _container?.Refresh();
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
            dropInfo.Item.Id,
            dropInfo.DropzoneIdentifier,
            dropInfo.IndexInZone + 1
        );
    }

    private async Task CreateList(string title)
    {
        await BoardState.CreateBoardListAsync(title);
    }

}