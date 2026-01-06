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

    public Task<UserBoard?> GetByUserAndBoard(Guid userId, Guid boardId)
    {
        return _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BoardId == boardId);
    }

    public async Task<UserBoardRole> GetRole(Guid userId, Guid boardId)
    {
        var userBoard = await GetByUserAndBoard(userId, boardId);
        if (userBoard is null)
        {
            return UserBoardRole.None;
        }
        return userBoard.Role;
    }
}
