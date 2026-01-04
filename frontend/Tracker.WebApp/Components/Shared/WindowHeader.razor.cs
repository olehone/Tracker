using Microsoft.AspNetCore.Components;

namespace Tracker.WebApp.Components.Shared;

public partial class WindowHeader
{
    [Parameter]
    public required string Title { get; set; }
}