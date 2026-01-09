using Tracker.Domain.Entities;

namespace Tracker.Application.Common.Repositories;

public interface IWorkspaceRepository : IRepository<Workspace, Guid>
{
    Task<IReadOnlyList<Workspace>> GetByUserAsync(Guid userId);
    Task<int> CountAsync(string? title = null,  Guid ? userId = null);
    Task<List<Workspace>> GetAsync(
        int skip, int take, string? title = null, Guid? userId = null);
    Task<int> CountMutualAsync(Guid targetUserId, Guid searchingUserId, string? title = null);
    Task<List<Workspace>> GetMutualAsync(
        Guid targetUserId, Guid searchingUserId, int skip, int take, string? title = null);
}
