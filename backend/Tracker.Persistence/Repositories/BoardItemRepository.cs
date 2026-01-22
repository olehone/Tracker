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

    public Task<List<BoardItem>> GetAssignedForUserAsync(Guid userId)
    {
        return _dbSet.AsNoTracking()
            .Where(bi => bi.Assignees
                .Any(bia => bia.BoardUser.UserId == userId))
            .ToListAsync();
    }

    public Task<List<BoardItem>> GetAssignedInBoardAsync(Guid boardUserId)
    {
        return _dbSet.AsNoTracking()
            .Where(bi => bi.Assignees
                .Any(bia => bia.BoardUser.Id == boardUserId))
            .ToListAsync();
    }

    public async Task<int> GetMaxPositionAsync(Guid boardListId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(bi => bi.BoardListId == boardListId)
            .MaxAsync(bi => (int?)bi.Position) ?? 0;
    }

    public Task ShiftPositionsAsync(Guid boardListId, int delta, int from)
    {
        return _dbSet
            .AsNoTracking()
            .Where(bi => bi.BoardListId == boardListId)
            .Where(bi => bi.Position >= from)
            .ExecuteUpdateAsync(bi => bi.SetProperty(bi => bi.Position, bi => bi.Position + delta));
    }

    public Task ShiftPositions(Guid boardListId, int delta, int from, int to)
    {
        return _dbSet
            .AsNoTracking()
            .Where(bi => bi.BoardListId == boardListId)
            .Where(bi => bi.Position >= from && bi.Position <= to)
            .ExecuteUpdateAsync(bi => bi.SetProperty(bi => bi.Position, bi => bi.Position + delta));
    }

}