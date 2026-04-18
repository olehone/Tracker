namespace Tracker.Domain.Requests.Roadmap;

public class SaveRoadmapRequest
{
    public List<SaveRoadmapNodeRequest> Nodes { get; set; } = [];
    public List<SaveRoadmapArrowRequest> Arrows { get; set; } = [];
}