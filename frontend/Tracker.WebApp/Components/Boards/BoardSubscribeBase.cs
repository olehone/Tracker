using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardSubscribeBase : ComponentBase, IDisposable
{
    [CascadingParameter]
    protected BoardState BoardState { get; set; } = null!;

    protected BoardFullDto Board => BoardState.Board;

    protected override void OnParametersSet()
    {
        BoardState.OnChange += StateHasChangedHandler;
    }

    protected virtual void StateHasChangedHandler()
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        BoardState.OnChange -= StateHasChangedHandler;
    }
}
