using Microsoft.EntityFrameworkCore;
using Tracker.Application.Common.Repositories;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Repositories;

public class WorkspaceRepository : Repository<Workspace, Guid>, IWorkspaceRepository
{

    public WorkspaceRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
    {
    }

    public new async Task<Workspace?> GetByIdAsync(Guid id)
    {
        return await _dbSet
           .AsNoTracking()
           .Include(x => x.PermissionRoles)
           .FirstOrDefaultAsync(x => x.Id == id);
    }

    public new void Update(Workspace workspace)
    {
        _dbSet.Attach(workspace);
        _dbContext.Entry(workspace).Property(w => w.Title).IsModified = true;
        _dbContext.Entry(workspace).Property(w => w.Description).IsModified = true;
        _dbContext.Entry(workspace).Property(w => w.Visibility).IsModified = true;
        var permissionRoles = _dbContext.Entry(workspace).Reference(w => w.PermissionRoles).TargetEntry;
        if (permissionRoles is not null)
        {
            permissionRoles.State = EntityState.Modified;
        }
    }

    public async Task<IReadOnlyList<Workspace>> GetByUserAsync(Guid userId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(w => w.UserWorkspaces.Any(uw => uw.UserId == userId))
            .ToListAsync();
    }

    private IQueryable<Workspace> SearchByTitleAndUserAsync(
        Guid? userId, string? title)
    {
        var query = _dbSet
            .AsNoTracking();
        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(w => EF.Functions.Like(w.Title, $"%{title}%"))
                .OrderBy(w => w.Title);
        }
        if (userId is not null)
        {
            query = query.Where(w =>
                    w.UserWorkspaces.Any(uw => uw.UserId == userId));
        }
        return query;
    }

    public async Task<int> CountAllAsync(string? title = null, Guid? userId = null)
    {
        return await SearchByTitleAndUserAsync(userId, title)
            .CountAsync();
    }

    public async Task<List<Workspace>> GetAllAsync(
        int skip, int take, string? title = null, Guid? userId = null)
    {
        return await SearchByTitleAndUserAsync(userId, title)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    private IQueryable<Workspace> SearchByTitleAndUsersAsync(
        Guid targetUserId, Guid searchingUserId, string? title = null)
    {
        return SearchByTitleAndUserAsync(targetUserId, title)
            .Where(w => w.UserWorkspaces.Any(uw => uw.UserId == searchingUserId));
    }

    public async Task<int> CountMutualAsync(
        Guid targetUserId, Guid searchingUserId, string? title = null)
    {
        return await SearchByTitleAndUsersAsync(targetUserId, searchingUserId, title)
            .CountAsync();
    }

    public async Task<List<Workspace>> GetMutualAsync(
        Guid targetUserId, Guid searchingUserId, int skip, int take, string? title = null)
    {
        return await SearchByTitleAndUsersAsync(targetUserId, searchingUserId, title)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }
}
