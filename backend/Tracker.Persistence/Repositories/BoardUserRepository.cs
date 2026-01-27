using Microsoft.EntityFrameworkCore;
using Tracker.Application.Common.Repositories;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;

namespace Tracker.Persistence.Repositories;

public class BoardUserRepository : Repository<BoardUser, Guid>, IBoardUserRepository
{

    public BoardUserRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
    {
    }

    public async Task<BoardUser?> GetAsync(Guid userId, Guid boardId)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BoardId == boardId);
    }

    public async Task<IReadOnlyList<BoardUser>> GetAsync(Guid boardId)
    {
        return await _dbSet.AsNoTracking()
            .Include(ub => ub.User)
            .Where(ub => ub.BoardId == boardId)
            .ToListAsync();
    }

    public async Task<BoardUser?> GetOwnerAsync(Guid boardId)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(ub => ub.BoardId == boardId && ub.Role == BoardUserRole.Owner);
    }

    public async Task<BoardUserRole> GetRoleAsync(Guid userId, Guid boardId)
    {
        var userBoard = await GetAsync(userId, boardId);
        if (userBoard is null)
        {
            return BoardUserRole.None;
        }
        return userBoard.Role;
    }

    public new async Task RemoveAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            var assignees = _dbContext.BoardItemAssignees.Where(bia => bia.BoardUserId == id);
            _dbContext.BoardItemAssignees.RemoveRange(assignees);
            _dbSet.Remove(entity);
        }
    }
}
