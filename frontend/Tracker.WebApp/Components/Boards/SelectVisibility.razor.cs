using Microsoft.AspNetCore.Components;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.Boards;
public partial class SelectVisibility
{
    [Parameter]
    public BoardVisibility Value { get; set; }
    [Parameter]
    public EventCallback<BoardVisibility> ValueChanged { get; set; }
    [Parameter]
    public required string Label { get; set; }
}