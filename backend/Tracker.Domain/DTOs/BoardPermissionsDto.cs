namespace Tracker.Domain.Dtos;

public class BoardPermissionsDto
{
    public static readonly BoardPermissionsDto None = new()
    {
        CanChangeBoard = false,
        CanCreateItem = false,
        CanChangeItem = false,
        CanCreateList = false,
        CanChangeList = false,
        CanChangeOwner = false,
    };

    public static readonly BoardPermissionsDto All = new()
    {
        CanChangeBoard = true,
        CanCreateItem = true,
        CanChangeItem = true,
        CanCreateList = true,
        CanChangeList = true,
        CanChangeOwner = true,
    };

    public required bool CanChangeBoard { get; set; }
    public required bool CanCreateItem { get; set; }
    public required bool CanChangeItem { get; set; }
    public required bool CanCreateList { get; set; }
    public required bool CanChangeList { get; set; }
    public required bool CanChangeOwner { get; set; }
}