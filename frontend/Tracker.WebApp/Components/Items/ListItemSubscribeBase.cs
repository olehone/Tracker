using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Items;

public partial class ListItemSubscribeBase : ComponentBase, IDisposable
{
    [CascadingParameter]
    protected BoardState BoardState { get; set; } = null!;

    protected IReadOnlyList<BoardItemDto> Items => BoardState.ItemsState.BoardItems;
    protected IReadOnlyList<BoardListDto> Lists => BoardState.ListsState.BoardLists;

    protected override void OnParametersSet()
    {
        BoardState.ListsState.OnChange += StateHasChangedHandler;
        BoardState.ItemsState.OnChange += StateHasChangedHandler;
    }

    protected virtual void StateHasChangedHandler()
    {
        InvokeAsync(StateHasChanged);
    }


    public void Dispose()
    {
        BoardState.ListsState.OnChange -= StateHasChangedHandler;
        BoardState.ItemsState.OnChange -= StateHasChangedHandler;
    }
}
