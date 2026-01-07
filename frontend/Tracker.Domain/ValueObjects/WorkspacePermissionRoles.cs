using Tracker.Domain.Enums;

namespace Tracker.Domain.ValueObjects;

public class WorkspacePermissionRoles
{
    public WorkspacePermissionRole MinCreateBoardRole { get; set; } 
        = WorkspacePermissionRole.Admin;
    public WorkspacePermissionRole MinChangeBoardRole { get; set; } 
        = WorkspacePermissionRole.Admin;
}