using Tracker.Domain.Entities.Common;

namespace Tracker.Domain.Entities;

public class BoardItem : BaseEntity
{
    public required Guid BoardListId { get; set; }
    public int Position { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public BoardList? BoardList { get; set; }
}
