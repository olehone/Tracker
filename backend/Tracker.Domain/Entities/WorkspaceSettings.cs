using Tracker.Domain.Enums;

namespace Tracker.Domain.Entities;

public class WorkspaceSettings
{
    public WorkspaceVisibility Visibility { get; set; } 
        = WorkspaceVisibility.Private;
    public WorkspacePermissionRole MinCreateBoardRole { get; set; } 
        = WorkspacePermissionRole.Admin;
}