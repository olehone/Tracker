namespace Tracker.API.Requests;

public class CreateBoardItemRequest
{
    public required Guid BoardListId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
}