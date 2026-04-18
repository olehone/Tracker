namespace Tracker.Domain.Requests.Roadmap;

public class SaveRoadmapArrowRequest
{
    public Guid SourceBoardItemId { get; set; }
    public Guid TargetBoardItemId { get; set; }
}
