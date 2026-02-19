using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Tracker.WebApp.Pages;

public partial class Call
{
    [Inject] IJSRuntime JS { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Connect();
        }
    }

    private async Task Connect()
    {
        await JS.InvokeVoidAsync("connect");
    }
    private async Task HangUpCall()
    {
        await JS.InvokeVoidAsync("hangUpCall");
    }
}