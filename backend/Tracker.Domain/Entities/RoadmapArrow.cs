using Tracker.Domain.Entities.Common;

namespace Tracker.Domain.Entities;

public class RoadmapArrow : BaseEntity
{
    public required Guid SourceNodeId { get; set; }
    public required Guid TargetNodeId { get; set; }

    public RoadmapNode Source { get; set; } = null!;
    public RoadmapNode Target { get; set; } = null!;
}
