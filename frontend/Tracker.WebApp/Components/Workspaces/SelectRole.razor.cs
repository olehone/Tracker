using Microsoft.AspNetCore.Components;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.Workspaces;
public partial class SelectRole
{
    [Parameter]
    public WorkspacePermissionRole Value { get; set; }

    [Parameter]
    public EventCallback<WorkspacePermissionRole> ValueChanged { get; set; }
    [Parameter]
    public required string Label { get; set; }
}