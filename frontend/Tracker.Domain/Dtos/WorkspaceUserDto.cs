using Tracker.Domain.Enums;

namespace Tracker.Domain.Dtos;

public class WorkspaceUserDto
{
    public required UserDto User { get; set; }
    public required UserWorkspaceRole Role { get; set; }
}