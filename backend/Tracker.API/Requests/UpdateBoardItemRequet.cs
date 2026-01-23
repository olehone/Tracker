using Tracker.Domain.Enums;

namespace Tracker.API.Requests;

public class UpdateBoardItemRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required bool IsDone { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public required BoardItemImportance Importance { get; set; }
}