using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardSubscribeBase : ComponentBase, IDisposable
{
    [CascadingParameter]
    protected BoardState BoardState { get; set; } = null!;

    protected BoardFullDto Board => BoardState.Board;

    private bool _disposed;

    protected override void OnParametersSet()
    {
        BoardState.OnChange += StateHasChangedHandler;
    }

    protected virtual void StateHasChangedHandler()
    {
        StateHasChanged();
    }

    protected virtual void InsideDispose()
    {
        BoardState.OnChange -= StateHasChangedHandler;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (BoardState != null)
        {
            InsideDispose();
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
