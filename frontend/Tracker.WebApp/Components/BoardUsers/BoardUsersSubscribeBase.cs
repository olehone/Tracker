using Microsoft.AspNetCore.Components;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardUsers;

public partial class BoardUsersSubscribeBase : ComponentBase, IDisposable
{
    [CascadingParameter]
    protected BoardState BoardState { get; set; } = null!;
    protected BoardUsersState UsersState => BoardState.UsersState;

    protected override void OnParametersSet()
    {
        BoardState.UsersState.OnChange += StateHasChangedHandler;
    }

    private void StateHasChangedHandler()
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        BoardState.UsersState.OnChange -= StateHasChangedHandler;
    }
}
