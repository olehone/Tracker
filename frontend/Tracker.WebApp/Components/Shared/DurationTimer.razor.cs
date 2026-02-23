using Microsoft.AspNetCore.Components;

namespace Tracker.WebApp.Components.Shared;

public partial class DurationTimer : IDisposable
{
    [Parameter, EditorRequired]
    public DateTimeOffset? StartTime { get; set; }

    private Timer? _timer;

    protected override void OnInitialized()
    {
        _timer = new Timer(_ =>
            InvokeAsync(StateHasChanged), null,
            TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}