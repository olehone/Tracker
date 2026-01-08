namespace Tracker.Domain.Dtos;

public class WorkspacePermissionsDto
{
    public required bool CanChangeWorkspace { get; set; }
    public required bool CanCreateBoard { get; set; }
    public required bool CanChangeBoard { get; set; }
}