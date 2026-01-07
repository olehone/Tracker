using Microsoft.EntityFrameworkCore;
using Tracker.Application.Common.Repositories;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;

namespace Tracker.Persistence.Repositories;

public class BoardRepository : Repository<Board, Guid>, IBoardRepository
{
    public BoardRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
    {
    }
    public async Task<Board?> GetByIdWithListsAndItemsAsync(Guid id)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(b => b.BoardLists
                .OrderBy(bl => bl.Position))
                .ThenInclude(bl => bl.BoardItems
                    .OrderBy(bi => bi.Position))
            .Where(b => b.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<Board>> GetPublicByWorkspaceAsync(Guid workspaceId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(b => b.WorkspaceId == workspaceId && b.Visibility == BoardVisibility.Public)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Board>> GetByWorkspaceAndUserAsync(Guid workspaceId, Guid userId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(b => b.WorkspaceId == workspaceId)
            .Include(b => b.UserBoards.Where(ub => ub.UserId == userId))
            .ToListAsync();
    }
}
