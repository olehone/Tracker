using Tracker.Domain.Enums;

namespace Tracker.API.Requests;

public record class UpdateBoardItemRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool? IsDone { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public bool ClearDueDate { get; set; }
    public BoardItemImportance? Importance { get; set; }
    public IReadOnlySet<Guid>? Assignees { get; set; }
}