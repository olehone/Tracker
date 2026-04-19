using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Components.Roadmap;

/// <summary>
/// A diagram node that wraps a board item.
/// Left/Right ports are added so arrows can be drawn between nodes.
/// </summary>
public class RoadmapItemNode : NodeModel
{
    public BoardItemDto Item { get; }

    public RoadmapItemNode(BoardItemDto item, Point? position = null)
        : base(position ?? Point.Zero)
    {
        Item = item;
        AddPort(PortAlignment.Left);
        AddPort(PortAlignment.Right);
    }
}
