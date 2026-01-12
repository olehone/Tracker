using Microsoft.AspNetCore.Components;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.Workspaces;
public partial class VisibilityChip
{
    [Parameter]
    public WorkspaceVisibility Visibility { get; set; }
}