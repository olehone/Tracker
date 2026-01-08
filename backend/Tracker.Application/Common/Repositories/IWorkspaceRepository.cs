using Tracker.Domain.Entities;

namespace Tracker.Application.Common.Repositories;

public interface IWorkspaceRepository : IRepository<Workspace, Guid>
{
    Task<IReadOnlyList<Workspace>> GetByUserAsync(Guid userId);
    Task<IReadOnlyList<Workspace>> SearchByTitleAndUserAsync(
        Guid userId, string title, int skip, int take);
}
