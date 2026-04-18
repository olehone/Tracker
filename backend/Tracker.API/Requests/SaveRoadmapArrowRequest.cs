namespace Tracker.API.Requests;

public class SaveRoadmapArrowRequest
{
    public Guid SourceBoardItemId { get; set; }
    public Guid TargetBoardItemId { get; set; }
}
