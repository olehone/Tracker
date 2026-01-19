using Tracker.Domain.Entities;

namespace Tracker.Application.Common.Repositories;

public interface IBoardListRepository : IRepository<BoardList, Guid>
{
    Task<int> GetMaxPositionAsync(Guid boardId);
    Task ShiftPositionsAsync(Guid boardId, int delta, int from, int to);

}
