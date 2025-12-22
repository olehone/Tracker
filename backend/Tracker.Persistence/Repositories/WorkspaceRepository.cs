using Tracker.Domain.Entities;
using Tracker.Application.Common.Repositories;

namespace Tracker.Persistence.Repositories;

public class WorkspaceRepository : Repository<Workspace, Guid>, IWorkspaceRepository
{

    public WorkspaceRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
    {
    }

}
