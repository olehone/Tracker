using Tracker.Domain.Entities;

namespace Tracker.Application.Common.Repositories;

public interface IBoardListRepository : IRepository<BoardList, Guid>
{
    Task<int> GetMaxPositionByBoardId(Guid boardId);
    Task ShiftPositions(Guid boardId, int delta, int from, int to);

}
