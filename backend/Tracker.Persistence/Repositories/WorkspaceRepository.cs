using Microsoft.EntityFrameworkCore;
using Tracker.Application.Common.Repositories;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;
using Tracker.Domain.ValueObjects;

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
           .Include(x => x.PermissionRoles)
           .FirstOrDefaultAsync(x => x.Id == id);
    }


    public async Task<Result> ChangePermissionRoles(Guid id, WorkspacePermissionRoles permissionRoles)
    {
        var updated = await _dbSet
            .Where(w => w.Id == id)
            .ExecuteUpdateAsync(u => u
                .SetProperty(w => w.PermissionRoles, permissionRoles));
        if (updated == 0)
        {
            return Error.NotFound("Workspace");
        }

        return Result.Success();
    }

    public async Task<Result> ChangeVisibility(Guid id, WorkspaceVisibility visibility)
    {
        var updated = await _dbSet
             .Where(w => w.Id == id)
             .ExecuteUpdateAsync(u => u
                 .SetProperty(w => w.Visibility, visibility));
        if (updated == 0)
        {
            return Error.NotFound("Workspace");
        }

        return Result.Success();
    }

    public async Task<Workspace?> GetByIdWithBoardsAsync(Guid id)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(w => w.Boards)
            .Where(w => w.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<Workspace>> GetByUserAsync(Guid userId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(w => w.UserWorkspaces.Any(uw => uw.UserId == userId))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Workspace>> SearchByTitleAndUserAsync(
        Guid userId, string title, int skip, int take)
    {
        var normalizedTitle = title.Trim().ToLower();

        return await _dbSet
            .AsNoTracking()
            .Where(w => w.Title.ToLower().Contains(normalizedTitle) &&
            (
                w.UserWorkspaces.Any(uw => uw.UserId == userId) ||
                w.Visibility == WorkspaceVisibility.Public
            ))
            .OrderByDescending(w => w.UserWorkspaces.Any(uw => uw.UserId == userId))
            .ThenBy(w => w.Title)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }
}
