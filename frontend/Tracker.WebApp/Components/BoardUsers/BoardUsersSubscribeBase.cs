using Microsoft.AspNetCore.Components;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardUsers;

public partial class BoardUsersSubscribeBase : ComponentBase, IDisposable
{
    [CascadingParameter]
    protected BoardState BoardState { get; set; } = null!;
    protected BoardUsersState UsersState => BoardState.UsersState;

    private bool _disposed;

    protected override void OnParametersSet()
    {
        BoardState.UsersState.OnChange += StateHasChangedHandler;
    }

    private void StateHasChangedHandler()
    {
        InvokeAsync(StateHasChanged);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (BoardState?.UsersState != null)
        {
            BoardState.UsersState.OnChange -= StateHasChangedHandler;
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
