using Tracker.Domain.Entities.Common;

namespace Tracker.Domain.Entities;

public class RoadmapNode : BaseEntity
{
    public required Guid BoardId { get; set; }
    public required Guid BoardItemId { get; set; }
    public double X { get; set; }
    public double Y { get; set; }

    public Board Board { get; set; } = null!;
    public BoardItem BoardItem { get; set; } = null!;
    public List<RoadmapArrow> OutgoingArrows { get; set; } = [];
    public List<RoadmapArrow> IncomingArrows { get; set; } = [];
}
