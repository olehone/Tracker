using Tracker.Domain.Entities.Common;
using Tracker.Domain.Enums;
using Tracker.Domain.ValueObjects;

namespace Tracker.Domain.Entities;

public class Workspace : BaseEntity
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public List<Board> Boards { get; set; } = [];
    public WorkspaceVisibility Visibility { get; set; }
        = WorkspaceVisibility.Private;
    public WorkspacePermissionRoles PermissionRoles { get; set; } = new();
    public List<UserWorkspace> UserWorkspaces { get; set; } = [];
}
