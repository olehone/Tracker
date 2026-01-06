using Tracker.Domain.Enums;

namespace Tracker.Domain.ValueObjects;

public class WorkspaceSettings
{
    public WorkspaceVisibility Visibility { get; set; } 
        = WorkspaceVisibility.Private;
    public WorkspacePermissionRole MinCreateBoardRole { get; set; } 
        = WorkspacePermissionRole.Admin;
    public WorkspacePermissionRole MinChangeBoardRole { get; set; } 
        = WorkspacePermissionRole.Admin;
}