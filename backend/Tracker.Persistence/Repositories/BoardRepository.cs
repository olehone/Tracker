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

    public Task<Board?> GetBoardWithWorkspaceAsync(Guid id)
    {
        return _dbSet
            .AsNoTracking()
            .Include(b => b.Workspace)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public Task<Board?> GetBoardWithWorkspaceByListAsync(Guid listId)
    {
        return _dbSet
            .AsNoTracking()
            .Include(b => b.Workspace)
            .FirstOrDefaultAsync(b =>
                b.BoardLists.Any(l => l.Id == listId));
    }

    public Task<Board?> GetBoardWithWorkspaceByItemAsync(Guid itemId)
    {
        return _dbSet
            .AsNoTracking()
            .Include(b => b.Workspace)
            .FirstOrDefaultAsync(b =>
                b.BoardLists.Any(l =>
                    l.BoardItems.Any(bi => bi.Id == itemId)));
    }

    public Task<Board?> GetByIdWithListsItemsUsersAsync(Guid id)
    {
        return _dbSet
            .AsNoTracking()
            .Include(b => b.BoardLists
                .OrderBy(bl => bl.Position))
                .ThenInclude(bl => bl.BoardItems
                    .OrderBy(bi => bi.Position))
            .Include(b => b.PermissionRoles)
            .Include(b => b.UserBoards)
                .ThenInclude(bu => bu.User)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public new void Update(Board board)
    {
        _dbSet.Attach(board);
        _dbContext.Entry(board).Property(b => b.Title).IsModified = true;
        _dbContext.Entry(board).Property(b => b.Description).IsModified = true;
        _dbContext.Entry(board).Property(b => b.Visibility).IsModified = true;
        var permissionRoles = _dbContext.Entry(board).Reference(b => b.PermissionRoles).TargetEntry;
        if (permissionRoles is not null)
        {
            permissionRoles.State = EntityState.Modified;
        }
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
