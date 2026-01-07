namespace Tracker.Domain.Dtos;

public class WorkspacePermissionsDto
{
    public static readonly WorkspacePermissionsDto None = new()
    {
        CanCreateBoard = false,
        CanChangeBoard = false,
        CanChangeSettings = false,
    };

    public static readonly WorkspacePermissionsDto All = new()
    {
        CanCreateBoard = true,
        CanChangeBoard = true,
        CanChangeSettings = true,
    };

    public required bool CanCreateBoard { get; set; }
    public required bool CanChangeBoard { get; set; }
    public required bool CanChangeSettings { get; set; }
}