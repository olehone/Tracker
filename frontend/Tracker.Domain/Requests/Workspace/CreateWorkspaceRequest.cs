namespace Tracker.Domain.Requests.Workspace;

public class CreateWorkspaceRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}