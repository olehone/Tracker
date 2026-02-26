using Domain.Enums;

namespace Domain.Entities;

public class BoardItem : BaseEntity
{
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public required Guid BoardListId { get; set; }
    public required int Position { get; set; }
    public bool IsDone { get; set; } = false;
    public DateTimeOffset? DueDate { get; set; }
    public BoardItemImportance Importance { get; set; } = BoardItemImportance.Low;
    public HashSet<BoardItemAssignee> Assignees { get; set; } = [];
    public HashSet<BoardItemAttachment> Attachments { get; set; } = [];
    public HashSet<ItemComment> Comments { get; set; } = [];
}
