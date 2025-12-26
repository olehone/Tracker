using Microsoft.EntityFrameworkCore;
using Tracker.Application.Common.Repositories;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Repositories;

public class BoardItemRepository : Repository<BoardItem, Guid>, IBoardItemRepository
{
    public BoardItemRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
    {
    }

    public async Task<int> GetMaxPositionByListIdAsync(Guid boardListId)
    {
        return await _dbSet
            .Where(bi => bi.BoardListId == boardListId)
            .MaxAsync(bi => (int?)bi.Position) ?? 0;
    }

    public async Task ShiftPositions(Guid boardListId, int delta, int from)
    {
        await _dbSet
            .Where(bi => bi.BoardListId == boardListId)
            .Where(bi => bi.Position >= from)
            .ExecuteUpdateAsync(bi => bi.SetProperty(bi => bi.Position, bi => bi.Position + delta));
    }

    public async Task ShiftPositions(Guid boardListId, int delta, int from, int to)
    {
        await _dbSet
            .Where(bi => bi.BoardListId == boardListId)
            .Where(bi => bi.Position >= from && bi.Position <= to)
            .ExecuteUpdateAsync(bi => bi.SetProperty(bi => bi.Position, bi => bi.Position + delta));
    }

}