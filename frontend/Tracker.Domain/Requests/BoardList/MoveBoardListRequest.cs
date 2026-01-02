namespace Tracker.Domain.Requests.BoardList;

public class MoveBoardListRequest
{
    public required Guid BoardListId { get; set; }
    public int Position { get; set; }
}