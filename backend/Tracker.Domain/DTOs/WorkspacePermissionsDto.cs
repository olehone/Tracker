namespace Tracker.Domain.Dtos;

public class WorkspacePermissionsDto
{
    public static readonly WorkspacePermissionsDto None = new()
    {
        CanCreateBoard = false,
        CanChangeBoard = false
    };

    public static readonly WorkspacePermissionsDto All = new()
    {
        CanCreateBoard = true,
        CanChangeBoard = true
    };

    public required bool CanCreateBoard { get; set; }
    public required bool CanChangeBoard { get; set; }
}