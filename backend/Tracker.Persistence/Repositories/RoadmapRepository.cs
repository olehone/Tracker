using Microsoft.EntityFrameworkCore;
using Tracker.Application.Common.Repositories;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Repositories;

public class RoadmapRepository(ApplicationDbContext applicationDbContext)
        : Repository<RoadmapNode, Guid>(applicationDbContext), IRoadmapRepository
{
    public async Task<RoadmapDto> GetByBoardIdAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        var nodes = await _dbSet
            .Where(n => n.BoardId == boardId)
            .Include(n => n.OutgoingArrows)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new RoadmapDto
        {
            Nodes = nodes.Select(n => new RoadmapNodeDto
            {
                Id = n.Id,
                BoardItemId = n.BoardItemId,
                X = n.X,
                Y = n.Y
            }).ToList(),

            Arrows = nodes
                .SelectMany(n => n.OutgoingArrows)
                .Select(a => new RoadmapArrowDto
                {
                    Id = a.Id,
                    SourceNodeId = a.SourceNodeId,
                    TargetNodeId = a.TargetNodeId,
                    SourceSide = a.SourceSide,
                    TargetSide = a.TargetSide
                }).ToList()
        };
    }

    public async Task DeleteByBoardIdAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        var nodes = await _dbSet
            .Where(n => n.BoardId == boardId)
            .Include(n => n.OutgoingArrows)
            .ToListAsync(cancellationToken);

        _dbContext.Set<RoadmapArrow>().RemoveRange(nodes.SelectMany(n => n.OutgoingArrows));
        _dbSet.RemoveRange(nodes);
    }

    public async Task AddArrowsAsync(IEnumerable<RoadmapArrow> arrows, CancellationToken cancellationToken = default)
    {
        await _dbContext.RoadmapArrows
            .AddRangeAsync(arrows, cancellationToken);
    }
}
