using Tracker.Domain.Entities.Common;

namespace Tracker.Domain.Entities;

public class Board : BaseEntity
{
    public required Guid WorkspaceId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public Workspace Workspace { get; set; } = null!;
    public List<BoardList> BoardLists { get; set; } = [];
    public BoardSettings Settings { get; set; } = new();
    public List<UserBoard> UserBoards { get; set; } = [];
}
