using Tracker.Domain.Enums;

namespace Tracker.Domain.Dtos;

public class BoardSummaryDto
{
    public required Guid Id { get; set; }
    public required Guid WorkspaceId { get; set; }
    public required string Title { get; set; }
    public required bool IsArchived { get; set; }
    public bool IsAbleToUnarchive { get; set; } = false;
    public bool IsParticipating { get; set; } = false;
    public required BoardVisibility Visibility { get; set; }
}