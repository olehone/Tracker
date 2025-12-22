using Tracker.Domain.Entities;

namespace Tracker.Domain.Dtos;

public class WorkspaceDto
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public List<BoardSummaryDto> Boards { get; set; } = [];
}
