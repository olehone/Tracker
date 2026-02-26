namespace Domain.Entities;

public class Board : BaseEntity
{
    public Guid WorkspaceId { get; set; }
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<BoardList> BoardLists { get; set; } = [];
    public BoardVisibility Visibility { get; set; }
        = BoardVisibility.Private;
    public BoardPermissionRoles PermissionRoles { get; set; } = new();

    public ArchiveStatus ArchiveStatus { get; set; } = ArchiveStatus.NotArchived;
}