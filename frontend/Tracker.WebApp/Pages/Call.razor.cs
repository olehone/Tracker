using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Pages;

public partial class Call : IDisposable
{
    private bool _showDeviceSettings = false;
    
    [Inject] CallState CallState { get; set; } = null!;
    [Inject] AppState AppState { get; set; } = null!;
    [Inject] IJSRuntime JS { get; set; } = null!;


    protected override async Task OnInitializedAsync()
    {
        CallState.OnChange += OnCallStateChanged;
        await CallState.InitializeAsync();

        if (AppState.IsAuthenticated && !CallState.IsInCall)
            await CallState.JoinAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JS.InvokeVoidAsync("attachLocalStream");

        if (CallState.IsInCall)
        {
            foreach (var userId in CallState.RemoteUsers)
                await JS.InvokeVoidAsync("attachRemoteStream", userId);

            foreach (var userId in CallState.RemoteScreenUsers)
                await JS.InvokeVoidAsync("attachRemoteScreenStream", userId);

            if (CallState.IsSharingLocalScreen)
                await JS.InvokeVoidAsync("attachLocalScreenStream");
        }
    }

    private async Task JoinAsync()
    {
        await CallState.JoinAsync();
    }

    private void OnCallStateChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        CallState.OnChange -= OnCallStateChanged;
    }
}
