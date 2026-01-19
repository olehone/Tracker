namespace Tracker.API.Requests;

public class MoveBoardItemRequest
{
    public required Guid ToBoardListId { get; set; }
    public required int Position { get; set; }
}
