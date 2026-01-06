using Microsoft.EntityFrameworkCore;
using Tracker.Application.Common.Repositories;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;

namespace Tracker.Persistence.Repositories;

public class UserWorkspaceRepository : Repository<UserWorkspace, Guid>, IUserWorkspaceRepository
{

    public UserWorkspaceRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
    {
    }

    public async Task<UserWorkspace?> GetByUserAndWorkspace(Guid userId, Guid workspaceId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(uw => uw.UserId == userId && uw.WorkspaceId == workspaceId);
    }

    public async Task<UserWorkspaceRole> GetRole(Guid userId, Guid workspaceId)
    {
        var userWorkspace = await GetByUserAndWorkspace(userId, workspaceId);
        if (userWorkspace is null)
        {
            return UserWorkspaceRole.None;
        }
        return userWorkspace.Role;
    }
}
