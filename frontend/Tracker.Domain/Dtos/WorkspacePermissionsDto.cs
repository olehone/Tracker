namespace Tracker.Domain.Dtos;

public class WorkspacePermissionsDto
{
    public required bool CanCreateBoardRole { get; set; }
    public required bool CanChangeBoardRole { get; set; }
}