using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction.Auth;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class Kanban
{
    private BoardFullDto Board => BoardState.Board!;
    private MudDropContainer<BoardItemDto> _container = null!;

    protected override void StateHasChangedHandler()
    {
        InvokeAsync(StateHasChanged);
        _container?.Refresh();
    }

    private bool ItemDisabled(BoardItemDto item)
    {
        if (BoardState.IsUnauthenticated)
        {
            return true;
        }
        if (Board.Permissions.CanChangeItem)
        {
            return false;
        }
        if (item.Assignees.Contains(BoardState.CurrentUserId))
        {
            return false;
        }
        return true;
    }

    private async Task ItemDropped(MudItemDropInfo<BoardItemDto> dropInfo)
    {
        if (dropInfo.Item is null)
        {
            return;
        }

        await BoardState.ItemsState.MoveAsync(dropInfo.Item.Id,
            dropInfo.DropzoneIdentifier,
            dropInfo.IndexInZone + 1);
    }

    private async Task CreateList(string title)
    {
        await BoardState.ListsState.CreateAsync(title);
    }
}