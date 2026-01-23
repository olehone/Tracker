namespace Tracker.API.Requests;

public class UpdateBoardListRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }
}
