using Microsoft.EntityFrameworkCore;
using Tracker.Application.Common.Repositories;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Repositories;

public class UserWorkspaceRepository : Repository<UserWorkspace, Guid>, IUserWorkspaceRepository
{

    public UserWorkspaceRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
    {
    }

    public async Task<UserWorkspace?> GetByUserAndWorkspaceIds(Guid userId, Guid workspaceId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(uw => uw.UserId == userId && uw.WorkspaceId == workspaceId);
    }
}
