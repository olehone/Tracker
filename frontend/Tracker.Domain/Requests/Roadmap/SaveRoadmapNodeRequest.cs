namespace Tracker.Domain.Requests.Roadmap;

public class SaveRoadmapNodeRequest
{
    public Guid BoardItemId { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}
