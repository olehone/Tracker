using Tracker.Domain.Entities;

namespace Tracker.Application.Common.Repositories;

public interface IBoardItemRepository : IRepository<BoardItem, Guid>
{
    Task<int> GetMaxPositionAsync(Guid boardListId);
    Task ShiftPositionsAsync(Guid boardListId, int delta, int from);
    Task ShiftPositions(Guid boardListId, int delta, int from, int to);
}
