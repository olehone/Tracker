namespace Tracker.Domain.Dtos;

public class BoardPermissionsDto
{
    public static readonly BoardPermissionsDto None = new()
    {
        CanCreateItem = false,
        CanChangeItem = false,
        CanCreateList = false,
        CanChangeList = false,
    };

    public static readonly BoardPermissionsDto All = new()
    {
        CanCreateItem = true,
        CanChangeItem = true,
        CanCreateList = true,
        CanChangeList = true,
    };

    public required bool CanCreateItem { get; set; }
    public required bool CanChangeItem { get; set; }
    public required bool CanCreateList { get; set; }
    public required bool CanChangeList { get; set; }
}