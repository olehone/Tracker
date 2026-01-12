using Microsoft.AspNetCore.Components;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.Workspaces;

public partial class WorkspaceVisibilityChip
{
    [Parameter]
    public WorkspaceVisibility Visibility { get; set; }
}