using Tracker.Domain.Enums;

namespace Tracker.Domain.Dtos;

public class RoadmapArrowDto
{
    public Guid Id { get; set; }
    public Guid SourceNodeId { get; set; }
    public Guid TargetNodeId { get; set; }
    public SideEnum SourceSide { get; set; }
    public SideEnum TargetSide { get; set; }
}
