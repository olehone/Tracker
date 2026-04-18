namespace Tracker.Domain.Dtos;

public class RoadmapDto
{
    public List<RoadmapNodeDto> Nodes { get; set; } = [];
    public List<RoadmapArrowDto> Arrows { get; set; } = [];
}
