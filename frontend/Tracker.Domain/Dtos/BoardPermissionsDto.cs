namespace Tracker.Domain.Dtos;

public class BoardPermissionsDto
{
    public required bool CanChangeBoard { get; set; }
    public required bool CanCreateItem { get; set; }
    public required bool CanChangeItem { get; set; }
    public required bool CanCreateList { get; set; }
    public required bool CanChangeList { get; set; }
    public required bool CanChangeOwner { get; set; }
    public required bool CanChangeArchiveStatus { get; set; }
    public required bool CanDeleteBoard { get; set; }
}