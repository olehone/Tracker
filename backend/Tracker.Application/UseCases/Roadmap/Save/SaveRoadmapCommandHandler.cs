using MediatR;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Entities;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Roadmap.Save;

public class SaveRoadmapCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<SaveRoadmapCommand, Result>
{
    public async Task<Result> Handle(SaveRoadmapCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        await uow.RoadmapRepository.DeleteByBoardIdAsync(request.BoardId, cancellationToken);

        var nodeByItemId = request.Nodes.ToDictionary(
            n => n.BoardItemId,
            n => new RoadmapNode
            {
                BoardId = request.BoardId,
                BoardItemId = n.BoardItemId,
                X = n.X,
                Y = n.Y
            });

        foreach (var node in nodeByItemId.Values)
        {
            await uow.RoadmapRepository.AddAsync(node);
        }

        var arrows = request.Arrows
            .Where(a => nodeByItemId.ContainsKey(a.SourceBoardItemId)
                     && nodeByItemId.ContainsKey(a.TargetBoardItemId))
            .Select(a => new RoadmapArrow
            {
                SourceNodeId = nodeByItemId[a.SourceBoardItemId].Id,
                TargetNodeId = nodeByItemId[a.TargetBoardItemId].Id,
                SourceSide = a.SourceSide,
                TargetSide = a.TargetSide
            });

        await uow.RoadmapRepository.AddArrowsAsync(arrows, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
