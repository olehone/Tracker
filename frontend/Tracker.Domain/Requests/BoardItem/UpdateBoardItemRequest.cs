using Tracker.Domain.Enums;

namespace Tracker.Domain.Requests.BoardItem;

public record class UpdateBoardItemRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool? IsDone { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public bool ClearDueDate { get; set; } = false;
    public BoardItemImportance? Importance { get; set; }
    public HashSet<Guid>? Assignees { get; set; }
}
