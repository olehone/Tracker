namespace Tracker.Domain.Requests.BoardItem;

public class CreateBoardItemRequest
{
    public required string Title { get; set; }
    public string? Description { get; set; }
}