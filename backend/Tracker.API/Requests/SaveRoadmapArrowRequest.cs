using Tracker.Domain.Enums;

namespace Tracker.API.Requests;

public class SaveRoadmapArrowRequest
{
    public Guid SourceBoardItemId { get; set; }
    public Guid TargetBoardItemId { get; set; }
    public SideEnum SourceSide { get; set; }
    public SideEnum TargetSide { get; set; }
}
