using Microsoft.EntityFrameworkCore;
using Tracker.Application.Common.Repositories;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;

namespace Tracker.Persistence.Repositories;

public class UserBoardRepository : Repository<UserBoard, Guid>, IUserBoardRepository
{

    public UserBoardRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
    {
    }

    public async Task<UserBoard?> GetAsync(Guid userId, Guid boardId)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BoardId == boardId);
    }

    public async Task<IReadOnlyList<UserBoard>> GetAsync(Guid boardId)
    {
        return await _dbSet.AsNoTracking()
            .Include(ub => ub.User)
            .Where(ub => ub.BoardId == boardId)
            .ToListAsync();
    }

    public async Task<UserBoard?> GetOwnerOfBoardAsync(Guid boardId)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(ub => ub.BoardId == boardId && ub.Role == UserBoardRole.Owner);
    }

    public async Task<UserBoardRole> GetRoleAsync(Guid userId, Guid boardId)
    {
        var userBoard = await GetAsync(userId, boardId);
        if (userBoard is null)
        {
            return UserBoardRole.None;
        }
        return userBoard.Role;
    }
}
