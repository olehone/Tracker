using Tracker.Domain.Entities.Common;

namespace Tracker.Domain.Entities;

public class BoardList : BaseEntity
{
    public required Guid BoardId { get; set; }
    public int Position { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required Board Board { get; set; }
    public List<BoardItem> BoardItems{ get; set; } = [];
}
