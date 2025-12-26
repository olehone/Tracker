namespace Tracker.API.Requests;

public class CreateBoardRequest
{
    public Guid WorkspaceId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
}
