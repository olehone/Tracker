using System.Runtime.Intrinsics.Arm;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Tracker.Services.Abstraction.Realtime;
using Tracker.Services.Abstraction.Realtime.Events;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Pages;

public partial class Call : IAsyncDisposable
{
    private bool _connected = false;
    private Guid currentCallId = Guid.Empty;
    private DotNetObjectReference<Call>? _objRef;

    public bool IsUnauthenticated => AppState.IsUnauthenticated;

    [Inject] IJSRuntime JS { get; set; } = null!;
    [Inject] ICallRealtimeService CallService { get; set; }
    [Inject] AppState AppState { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _objRef = DotNetObjectReference.Create(this);
            await JS.InvokeVoidAsync("registerDotNetInstance", _objRef);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        AppState.OnUserChange += OnAppStateChanged;
    }

    private async void OnAppStateChanged()
    {
        if (!IsUnauthenticated && !_connected)
        {
            _connected = true;
            await ConnectRealtimeAsync();
        }
    }

    private async Task Connect()
    {
        await ConnectRealtimeAsync();
    }
    private async Task HangUpCall()
    {
        await JS.InvokeVoidAsync("hangUpCall");
    }

    private async Task ConnectRealtimeAsync()
    {
        if (IsUnauthenticated)
        {
            return;
        }

        CallService.OnDataReceived += HandleReceivedData;
        CallService.OnVideoOffer += HandleVideoOffer;
        await CallService.ConnectAsync(currentCallId);
    }

    private void HandleReceivedData(string data)
    {
        JS.InvokeVoidAsync("handleReceiveData", data, AppState.MyId);
    }

    [JSInvokable]
    public Task SendToServer(string data)
    {
        return CallService.SendData(data);
    }
 
    private void HandleVideoOffer(VideoOfferEvent evt)
    {
        JS.InvokeVoidAsync("handleVideoOffer", evt.CallerId, evt.SessionDescriptionProtocol);
    }

    [JSInvokable]
    public Task SendOffer(Guid callerId, string sessionDescriptionProtocol)
    {
        return CallService.;
    }

    public async ValueTask DisposeAsync()
    {
        AppState.OnUserChange -= OnAppStateChanged;
        CallService.OnDataReceived -= HandleReceivedData;
        _objRef?.Dispose();
        await CallService.DisconnectAsync();
    }
}