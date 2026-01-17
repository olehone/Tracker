namespace Tracker.Domain.Dtos;

public class WorkspacePermissionsDto
{
    public static readonly WorkspacePermissionsDto None = new()
    {
        CanCreateBoard = false,
        CanChangeBoard = false,
        CanChangeWorkspace = false,
    };

    public required bool CanCreateBoard { get; set; }
    public required bool CanChangeBoard { get; set; }
    public required bool CanChangeWorkspace { get; set; }
}