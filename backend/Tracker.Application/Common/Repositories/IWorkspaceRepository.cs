using Tracker.Domain.Entities;

namespace Tracker.Application.Common.Repositories;

public interface IWorkspaceRepository : IRepository<Workspace, Guid>
{
    Task<Workspace?> GetByIdWithBoardsAsync(Guid id);
    Task<IReadOnlyList<Workspace>> GetByUserIdAsync(Guid userId);
    Task<IReadOnlyList<Workspace>> SearchByTitleWithUserIdAsync(
        Guid userId, string title, int skip, int take);
}
