namespace Tracker.API.Requests;

public class CreateBoardRequest
{
    public required Guid WorkspaceId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
}
