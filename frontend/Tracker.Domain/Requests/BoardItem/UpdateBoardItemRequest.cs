using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;

namespace Tracker.Domain.Requests.BoardItem;

public record class UpdateBoardItemRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool? IsDone { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public BoardItemImportance? Importance { get; set; }
    public HashSet<Guid>? Assignees { get; set; }

    public UpdateBoardItemRequest() : base() { }
    public UpdateBoardItemRequest(BoardItemDto original, BoardItemDto changed)
    {
        if (original.Title != changed.Title)
        {
            Title = changed.Title;
        }
        if (original.Description != changed.Description)
        {
            Description = changed.Description;
        }
        if (original.IsDone != changed.IsDone)
        {
            IsDone = changed.IsDone;
        }
        if (original.DueDate != changed.DueDate)
        {
            DueDate = changed.DueDate;
        }
        if (original.Importance != changed.Importance)
        {
            Importance = changed.Importance;
        }
        if (!original.Assignees.SetEquals(changed.Assignees))
        {
            Assignees = changed.Assignees;
        }
    }
}
