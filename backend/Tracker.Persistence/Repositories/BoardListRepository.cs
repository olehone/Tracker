using Microsoft.EntityFrameworkCore;
using Tracker.Application.Common.Repositories;
using Tracker.Domain.Entities;
using Tracker.Domain.Results;

namespace Tracker.Persistence.Repositories;

public class BoardListRepository : Repository<BoardList, Guid>, IBoardListRepository
{
    public BoardListRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
    {
    }

    public async Task<int> GetMaxPositionByBoardId(Guid boardId)
    {
        return await _dbSet
            .Where(bl => bl.BoardId == boardId)
            .MaxAsync(bl => (int?)bl.Position) ?? 0;
    }

    public async Task ShiftPositions(Guid boardId, int delta, int from, int to)
    {
        await _dbSet
            .Where(bl => bl.BoardId == boardId)
            .Where(bl => bl.Position >= from && bl.Position <= to)
            .ExecuteUpdateAsync(bi => bi.SetProperty(bi => bi.Position, bi => bi.Position + delta));
    }
}