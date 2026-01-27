using Microsoft.EntityFrameworkCore;
using Tracker.Application.Common.Repositories;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;

namespace Tracker.Persistence.Repositories;

public class WorkspaceUserRepository : Repository<WorkspaceUser, Guid>, IWorkspaceUserRepository
{

    public WorkspaceUserRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
    {
    }

    public async Task<WorkspaceUser?> GetAsync(Guid userId, Guid workspaceId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(uw => uw.UserId == userId && uw.WorkspaceId == workspaceId);
    }

    public async Task<IReadOnlyList<WorkspaceUser>> GetAsync(Guid workspaceId)
    {
        return await _dbSet.AsNoTracking()
            .Include(uw => uw.User)
            .Where(uw => uw.WorkspaceId == workspaceId)
            .ToListAsync();
    }

    public async Task<WorkspaceUser?> GetOwnerAsync(Guid workspaceId)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(uw => uw.WorkspaceId == workspaceId && uw.Role == WorkspaceUserRole.Owner);
    }

    public async Task<WorkspaceUserRole> GetRoleAsync(Guid userId, Guid workspaceId)
    {
        var userWorkspace = await GetAsync(userId, workspaceId);
        if (userWorkspace is null)
        {
            return WorkspaceUserRole.None;
        }
        return userWorkspace.Role;
    }
}
