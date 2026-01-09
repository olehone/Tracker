using Tracker.Domain.Enums;

namespace Tracker.Domain.Dtos;

public class WorkspaceMemberDto
{
    public required Guid UserId { get; set; }
    public required string Username { get; set; }
    public string? AvatarUrl { get; set; }
    public required UserWorkspaceRole Role { get; set; }
}