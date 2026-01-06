using Microsoft.EntityFrameworkCore;
using Tracker.Application.Common.Repositories;
using Tracker.Domain.Entities;

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
}
