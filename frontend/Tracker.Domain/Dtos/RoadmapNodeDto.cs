namespace Tracker.Domain.Dtos;

public class RoadmapNodeDto
{
    public Guid Id { get; set; }
    public Guid BoardItemId { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}
