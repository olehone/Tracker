using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardItems;

public partial class ListItemSubscribeBase : ComponentBase, IDisposable
{
    [CascadingParameter]
    protected BoardState BoardState { get; set; } = null!;

    protected IReadOnlyList<BoardItemDto> Items => BoardState.ItemsState.BoardItems;
    protected IReadOnlyList<BoardListDto> Lists => BoardState.ListsState.BoardLists;

    private bool _disposed;

    protected override void OnParametersSet()
    {
        BoardState.ListsState.OnChange += StateHasChangedHandler;
        BoardState.ItemsState.OnChange += StateHasChangedHandler;
    }

    protected void StateHasChangedHandler()
    {
        InvokeAsync(StateHasChanged);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (BoardState != null)
        {
            BoardState.ListsState.OnChange -= StateHasChangedHandler;
            BoardState.ItemsState.OnChange -= StateHasChangedHandler;
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
