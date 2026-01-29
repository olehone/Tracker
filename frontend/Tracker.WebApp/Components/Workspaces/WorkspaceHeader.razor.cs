using Microsoft.AspNetCore.Components;

namespace Tracker.WebApp.Components.Workspaces;

public partial class WorkspaceHeader
{
    [Parameter, EditorRequired]
    public string Title { get; set; }
    [Parameter, EditorRequired]
    public Guid Id { get; set; }
}