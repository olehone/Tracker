using Tracker.Domain.Enums;

namespace Tracker.API.Requests;

public class WorkspaceUserRoleRequest
{
    public required UserWorkspaceRole Role { get; set; }
}