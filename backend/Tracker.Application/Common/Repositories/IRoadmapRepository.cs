using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;

namespace Tracker.Application.Common.Repositories;

public interface IRoadmapRepository : IRepository<RoadmapNode, Guid>
{
    Task<RoadmapDto> GetByBoardIdAsync(Guid boardId, CancellationToken cancellationToken = default);
    Task DeleteByBoardIdAsync(Guid boardId, CancellationToken cancellationToken = default);
    Task AddArrowsAsync(IEnumerable<RoadmapArrow> arrows, CancellationToken cancellationToken = default);
}
