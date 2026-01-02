using Tracker.Domain.Entities;

namespace Tracker.Domain.Dtos;

public class BoardSummaryDto
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
}