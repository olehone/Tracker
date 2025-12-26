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

    public async Task<Workspace?> GetByIdWithBoardsAsync(Guid id)
    {
        return await _dbSet
            .Include(w => w.Boards)
            .Where(w => w.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<Workspace>> GetAllWithBoardsAsync()
    {
        return await _dbSet
            .Include(w => w.Boards)
            .ToListAsync();
    }
}
