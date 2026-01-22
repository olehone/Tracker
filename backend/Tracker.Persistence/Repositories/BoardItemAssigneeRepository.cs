using Microsoft.EntityFrameworkCore;
using Tracker.Application.Common.Repositories;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Repositories;

public class BoardItemAssigneeRepository : Repository<BoardItemAssignee, Guid>, IBoardItemAssigneeRepository
{
    public BoardItemAssigneeRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
    {
    }

    public async Task<BoardItemAssignee?> GetByUserAndItemAsync(Guid userId, Guid itemId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(bia => bia.BoardItemId == itemId &&
                bia.BoardUser.UserId == userId);
    }
}