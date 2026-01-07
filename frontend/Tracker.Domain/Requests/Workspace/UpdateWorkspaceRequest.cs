using Tracker.Domain.Enums;
using Tracker.Domain.ValueObjects;

namespace Tracker.Domain.Requests.Workspace;

public class UpdateWorkspaceRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required WorkspaceVisibility Visibility { get; set; }
    public required WorkspacePermissionRoles PermissionRoles { get; set; }
}