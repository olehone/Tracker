namespace Tracker.API.Requests;

public class CreateBoardListRequest
{
    public required Guid BoardId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
}
