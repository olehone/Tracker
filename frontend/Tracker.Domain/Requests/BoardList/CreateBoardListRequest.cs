namespace Tracker.Domain.Requests.BoardList;

public class CreateBoardListRequest
{
    public required Guid BoardId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}
