using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Realtime;

namespace Tracker.WebApp.Components.Call;

public partial class PeekingCall
{
    [Parameter, EditorRequired]
    public Guid CallId { get; set; }
    [Parameter, EditorRequired]
    public EventCallback OnEnded { get; set; }

    public event Action? OnCallEnded;

    private CallDto? Call { get; set; }

    [Inject] ICallRealtimeService CallStateService { get; set; } = null!;
    [Inject] ICallService CallService { get; set; } = null!;
    [Inject] NavigationManager Nav { get; set; } = null!;

    protected override async Task OnParametersSetAsync()
    {
        var result = await CallService.GetByIdAsync(CallId);
        if (result.IsSuccess)
        {
            Call = result.Value;
            OnCallEnded += HandleCallEnded;
        }
    }

    private void OpenCall()
    {
        Nav.NavigateTo($"/calls/{CallId}");
    }

    public void HandleCallEnded()
    {
        if (OnEnded.HasDelegate)
        {
            OnEnded.InvokeAsync();
        }
    }

    public void Dispose()
    {
        OnCallEnded -= HandleCallEnded;
    }
}