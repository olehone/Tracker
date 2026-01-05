namespace Tracker.Domain.Requests.BoardItem;

public class CreateBoardItemRequest
{
    public required Guid BoardListId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
}