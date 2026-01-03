namespace Tracker.API.Requests;

public class MoveBoardListRequest
{
    public required Guid BoardListId { get; set; }
    public required int Position { get; set; }
}
