using Tracker.Domain.Entities.Common;

namespace Tracker.Domain.Entities;

public class Workspace: BaseEntity
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public List<Board> Boards { get; set; } = [];

}
