using Tracker.Domain.Enums;

namespace Tracker.Domain.Dtos;

public class WorkspaceSummaryDto
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required WorkspaceVisibility Visibility { get; set; }
    public required string Description { get; set; }
}
