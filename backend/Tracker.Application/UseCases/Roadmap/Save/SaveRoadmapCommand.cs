using MediatR;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Roadmap.Save;

public class SaveRoadmapCommand : IRequest<Result>
{
    public required Guid BoardId { get; set; }
    public List<SaveRoadmapNodeCommand> Nodes { get; set; } = [];
    public List<SaveRoadmapArrowCommand> Arrows { get; set; } = [];
}

public class SaveRoadmapNodeCommand
{
    public Guid BoardItemId { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}

public class SaveRoadmapArrowCommand
{
    public Guid SourceBoardItemId { get; set; }
    public Guid TargetBoardItemId { get; set; }
    public SideEnum SourceSide { get; set; }
    public SideEnum TargetSide { get; set; }
}
