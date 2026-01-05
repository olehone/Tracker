using Microsoft.EntityFrameworkCore;
using Tracker.Application.Common.Repositories;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;

namespace Tracker.Persistence.Repositories;

public class WorkspaceRepository : Repository<Workspace, Guid>, IWorkspaceRepository
{

    public WorkspaceRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
    {
    }

    public async Task<Workspace?> GetByIdWithBoardsAsync(Guid id)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(w => w.Boards)
            .Where(w => w.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<Workspace>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(w => w.UserWorkspaces.Any(uw => uw.UserId == userId))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Workspace>> GetByUserIdAndQueryWithPublicAsync(Guid userId,
        string query, int skip, int take)
    {
        var normalizedQuery = query.Trim().ToLower();

        return await _dbSet
            .AsNoTracking()
            .Where(w => w.Title.ToLower().Contains(normalizedQuery) &&
            (
                w.UserWorkspaces.Any(uw => uw.UserId == userId) ||
                w.Settings.Visibility == WorkspaceVisibility.Public
            ))
            .OrderByDescending(w => w.UserWorkspaces.Any(uw => uw.UserId == userId))
            .ThenBy(w => w.Title)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }
}
