namespace Tracker.API.Requests;

public class CreateBoardItemRequest
{
    public required string Title { get; set; }
    public string? Description { get; set; }
}