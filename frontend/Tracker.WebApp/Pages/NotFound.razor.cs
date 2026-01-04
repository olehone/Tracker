using Microsoft.AspNetCore.Components;

namespace Tracker.WebApp.Pages;

public partial class NotFound
{
    [Parameter]
    public string Path { get; set; } = null!;
}