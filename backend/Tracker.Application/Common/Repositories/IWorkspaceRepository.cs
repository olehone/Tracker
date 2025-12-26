using Tracker.Domain.Entities;

namespace Tracker.Application.Common.Repositories;

public interface IWorkspaceRepository : IRepository<Workspace, Guid>
{
    Task<Workspace?> GetByIdWithBoardsAsync(Guid id);
    Task<IReadOnlyList<Workspace>> GetAllWithBoardsAsync();
}
