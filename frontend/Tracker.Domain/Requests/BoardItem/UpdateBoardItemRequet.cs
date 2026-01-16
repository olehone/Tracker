namespace Tracker.Domain.Requests.BoardItem;

public class UpdateBoardItemRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }
}
