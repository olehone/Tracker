using Tracker.Domain.Enums;

namespace Tracker.Domain.Entities;

public class UserWorkspace
{
    public required Guid UserId { get; set; }
    public required Guid WorkspaceId { get; set; }
    public required UserWorkspaceRole Role { get; set; }
    public User User { get; set; } = null!;
    public Workspace Workspace { get; set; } = null!;
}