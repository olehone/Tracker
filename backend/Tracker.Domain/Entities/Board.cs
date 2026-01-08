using Tracker.Domain.Entities.Common;
using Tracker.Domain.Enums;
using Tracker.Domain.ValueObjects;

namespace Tracker.Domain.Entities;

public class Board : BaseEntity
{
    public required Guid WorkspaceId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public Workspace Workspace { get; set; } = null!;
    public List<BoardList> BoardLists { get; set; } = [];
    public BoardVisibility Visibility { get; set; }
        = BoardVisibility.Private;
    public BoardPermissionRoles PermissionRoles { get; set; } = new();
    public List<UserBoard> UserBoards { get; set; } = [];
}
