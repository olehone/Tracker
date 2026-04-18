namespace Tracker.API.Requests;

public class SaveRoadmapRequest
{
    public List<SaveRoadmapNodeRequest> Nodes { get; set; } = [];
    public List<SaveRoadmapArrowRequest> Arrows { get; set; } = [];
}