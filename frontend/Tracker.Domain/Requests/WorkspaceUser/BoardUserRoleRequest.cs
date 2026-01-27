using Tracker.Domain.Enums;

namespace Tracker.Domain.Requests.WorkspaceUser;

public class WorkspaceUserRoleRequest
{
    public required WorkspaceUserRole Role { get; set; }

    public static implicit operator WorkspaceUserRoleRequest(WorkspaceUserRole role)
    {
        return new WorkspaceUserRoleRequest { Role = role };
    }
}