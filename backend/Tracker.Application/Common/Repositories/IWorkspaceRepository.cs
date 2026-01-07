using Tracker.Domain.Entities;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;
using Tracker.Domain.ValueObjects;

namespace Tracker.Application.Common.Repositories;

public interface IWorkspaceRepository : IRepository<Workspace, Guid>
{
    Task<Workspace?> GetByIdWithBoardsAsync(Guid id);
    Task<IReadOnlyList<Workspace>> GetByUserAsync(Guid userId);
    Task<IReadOnlyList<Workspace>> SearchByTitleAndUserAsync(
        Guid userId, string title, int skip, int take);
}
