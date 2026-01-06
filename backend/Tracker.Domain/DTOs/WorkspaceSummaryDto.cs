namespace Tracker.Domain.Dtos;

public class WorkspaceSummaryDto
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
}
