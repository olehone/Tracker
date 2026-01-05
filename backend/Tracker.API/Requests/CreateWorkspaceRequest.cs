namespace Tracker.API.Requests;

public class CreateWorkspaceRequest
{
    public required string Title { get; set; }
    public string? Description { get; set; }
}
