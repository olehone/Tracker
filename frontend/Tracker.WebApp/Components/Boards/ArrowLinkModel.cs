using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models;

public class ArrowLinkModel : LinkModel
{
    public ArrowLinkModel(PortModel source, PortModel target) : base(source, target)
    {
        SourceMarker = LinkMarker.Circle;
        TargetMarker = LinkMarker.Arrow;
    }
    public ArrowLinkModel(Anchor source, Anchor? target = null)
    : base(source, target)
    {
        SourceMarker = LinkMarker.Circle;
        TargetMarker = LinkMarker.Arrow;
    }
}