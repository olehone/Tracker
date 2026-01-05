using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.WebApp.Shared;

namespace Tracker.WebApp.Components.Workspaces;

public partial class WorkspaceNavGroup
{
    [Parameter]
    public required WorkspaceDto Workspace { get; set; }

    private string Title()
    {
        return UiHelper.ShortenText(Workspace.Title, 30);
    }
}