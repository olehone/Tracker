using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Pages;

public partial class Call : IDisposable
{
    [Inject] CallState CallState { get; set; } = null!;
    [Inject] AppState AppState { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        CallState.OnChange += OnCallStateChanged;
        AppState.OnUserChange += OnCallStateChanged;
        await CallState.InitializeAsync();
        await CallState.JoinAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // Only re-attach on first render. Subsequent re-renders are triggered by
        // state changes (Notify) and attachment is handled explicitly at the call
        // site (OnRemoteTrack, OnRemoteScreenTrack, ToggleScreenShareAsync etc.)
        // Running AttachStreamsAsync on every render causes getLocalStream() to be
        // called repeatedly, which contributes to spurious onnegotiationneeded events.
        if (firstRender)
            await CallState.AttachStreamsAsync();
    }

    private void OnCallStateChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        CallState.OnChange -= OnCallStateChanged;
        AppState.OnUserChange -= OnCallStateChanged;
    }
}