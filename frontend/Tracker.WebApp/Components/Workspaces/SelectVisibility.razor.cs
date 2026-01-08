using Microsoft.AspNetCore.Components;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.Workspaces;
public partial class SelectVisibility
{
    [Parameter]
    public WorkspaceVisibility Value { get; set; }
    [Parameter]
    public EventCallback<WorkspaceVisibility> ValueChanged { get; set; }
    [Parameter]
    public required string Label { get; set; }
}