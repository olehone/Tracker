namespace Tracker.API.Requests;

public class CreateBoardItemRequest
{
    public required Guid BoardListId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}
