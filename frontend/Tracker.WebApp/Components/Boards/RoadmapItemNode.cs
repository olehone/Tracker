using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Components.Boards;

public class RoadmapItemNode : NodeModel
{
    public BoardItemDto Item { get; }

    public PortModel TopPort { get; }
    public PortModel BottomPort { get; }
    public PortModel LeftPort { get; }
    public PortModel RightPort { get; }

    public RoadmapItemNode(BoardItemDto item, Point? position = null)
        : base(position ?? Point.Zero)
    {
        Item = item;

        TopPort = AddPort(PortAlignment.Top);
        BottomPort = AddPort(PortAlignment.Bottom);
        LeftPort = AddPort(PortAlignment.Left);
        RightPort = AddPort(PortAlignment.Right);
    }
}