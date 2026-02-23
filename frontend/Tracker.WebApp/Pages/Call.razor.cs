using Microsoft.AspNetCore.Components;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Pages;

public partial class Call : IAsyncDisposable
{
    [Inject] CallState CallState { get; set; } = null!;
    [Inject] AppState AppState { get; set; } = null!;
    [Inject] NavigationManager Nav { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        CallState.OnChange += OnCallStateChanged;
        CallState.OnLeaveCall += LeavePage;
        AppState.OnUserChange += OnCallStateChanged;
        await CallState.InitializeAsync();
        await CallState.JoinAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await CallState.AttachStreamsAsync();
    }

    private void OnCallStateChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    public Task LeaveCall()
    {
        return CallState.HangUpAsync();
    }

    public void LeavePage()
    {
        Nav.NavigateTo("/");
    }

    public async ValueTask DisposeAsync()
    {
        CallState.OnChange -= OnCallStateChanged;
        AppState.OnUserChange -= OnCallStateChanged;
    }
}