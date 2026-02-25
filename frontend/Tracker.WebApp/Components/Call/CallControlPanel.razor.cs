using Microsoft.AspNetCore.Components;
using Tracker.Services.Abstraction;
using Tracker.WebApp.Pages;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Call;

public partial class CallControlPanel : IDisposable
{
    [Inject] CallState CallState { get; set; } = null!;
    [Inject] NavigationManager Nav { get; set; } = null!;

    private void OpenCall()
    {
        Nav.NavigateTo($"/calls/{CallState.Call.Id}");
    }
    protected override void OnParametersSet()
    {
        CallState.OnChange += StateHasChanged;
    }

    public void Dispose()
    {
        CallState.OnChange -= StateHasChanged;
    }
}