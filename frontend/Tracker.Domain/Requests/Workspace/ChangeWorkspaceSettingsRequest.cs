using Tracker.Domain.Enums;

namespace Tracker.Domain.Requests.Workspace;

public class ChangeWorkspaceSettingsRequest
{
    public required Guid WorkspaceId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required WorkspaceVisibility Visibility { get; set; } 
    public required WorkspacePermissionRole MinCreateBoardRole { get; set; } 
    public required WorkspacePermissionRole MinChangeBoardRole { get; set; } 
}