using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Components.Boards;

public class RoadmapItemNode : NodeModel
{
    public BoardItemDto Item { get; }

    public RoadmapItemNode(BoardItemDto item, Point? position = null)
        : base(position ?? Point.Zero)
    {
        Item = item;
        AddPort(PortAlignment.Top);
        AddPort(PortAlignment.Bottom);
        AddPort(PortAlignment.Left);
        AddPort(PortAlignment.Right);
    }
}
