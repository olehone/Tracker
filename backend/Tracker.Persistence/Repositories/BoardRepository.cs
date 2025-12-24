using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Tracker.Application.Common.Repositories;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Repositories;

public class BoardRepository : Repository<Board, Guid>, IBoardRepository
{
    public BoardRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
    {
    }

    public async Task<List<Board>> GetAllByWorkspaceIdAsync(Guid workspaceId)
    {
        return await _dbSet
            .Where(b => b.WorkspaceId == workspaceId)
            .ToListAsync();
    }

    public async Task<Board?> GetByIdWithListsAndItemsAsync(Guid id)
    {
        return await _dbSet
            .Include(b => b.BoardLists
                .OrderBy(bl => bl.Position))
                .ThenInclude(bl => bl.BoardItems
                    .OrderBy(bi => bi.Position))
            .Where(b => b.Id == id)
            .FirstOrDefaultAsync();
    }

}
