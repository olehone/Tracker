using Microsoft.AspNetCore.Components;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Call;

public partial class CallControlPanel : IDisposable
{
    [Parameter]
    public bool ShowLinkToPage { get; set; } = true;

    [Inject] CallState CallState { get; set; } = null!;
    [Inject] NavigationManager Nav { get; set; } = null!;

    private void OpenCall()
    {
        Nav.NavigateTo($"/calls/{CallState.Call.Id}");
    }
    protected override void OnInitialized()
    {
        CallState.OnChange += StateHasChanged;
    }

    public void Dispose()
    {
        CallState.OnChange -= StateHasChanged;
    }
}