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
    private Guid _callId = Guid.Parse("29063d2a-7bfb-4384-84b7-0f8625677b0b"); // hardcoded until board wires it
    private DotNetObjectReference<Call>? _objRef;

    public bool IsUnauthenticated => AppState.IsUnauthenticated;

    [Inject] IJSRuntime JS { get; set; } = null!;
    [Inject] ICallRealtimeService CallService { get; set; } = null!;
    [Inject] AppState AppState { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;
        _objRef = DotNetObjectReference.Create(this);
        await JS.InvokeVoidAsync("registerDotNetInstance", _objRef);
    }

    protected override async Task OnInitializedAsync()
    {
        AppState.OnUserChange += OnAppStateChanged;

        if (!IsUnauthenticated)
        {
            await ConnectRealtimeAsync();
        }
    }

    private async void OnAppStateChanged()
    {
        if (!IsUnauthenticated && !_connected)
        {
            _connected = true;
            await ConnectRealtimeAsync();
        }
    }

    private async Task ConnectRealtimeAsync()
    {
        CallService.OnDataReceived += HandleReceivedData;
        await CallService.ConnectAsync(_callId);
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

    private async Task HangUpCall()
    {
        await JS.InvokeVoidAsync("hangUpCall");
    }

    public async ValueTask DisposeAsync()
    {
        AppState.OnUserChange -= OnAppStateChanged;
        CallService.OnDataReceived -= HandleReceivedData;
        _objRef?.Dispose();
        await CallService.DisconnectAsync();
    }
}