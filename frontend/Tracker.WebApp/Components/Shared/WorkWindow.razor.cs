using Microsoft.AspNetCore.Components;

namespace Tracker.WebApp.Components.Shared;

public partial class WorkWindow
{
    [Parameter]
    public required RenderFragment Header { get; set; }
    [Parameter]
    public required RenderFragment Body { get; set; }
    [Parameter]
    public int Elevation { get; set; } = 3;
}