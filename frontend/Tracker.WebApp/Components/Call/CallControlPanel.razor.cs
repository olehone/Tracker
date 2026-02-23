using Microsoft.AspNetCore.Components;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Call;

public partial class CallControlPanel : IDisposable
{
    [Inject] CallState CallState { get; set; } = null!;

    protected override void OnParametersSet()
    {
        CallState.OnChange += StateHasChanged;
    }

    public void Dispose()
    {
        CallState.OnChange -= StateHasChanged;
    }
}