using Tracker.Domain.Entities;

namespace Tracker.Application.Common.Repositories;

public interface IBoardRepository : IRepository<Board, Guid>
{
    Task<Board?> GetByIdWithListsAndItemsAsync(Guid id);
    Task<IReadOnlyList<Board>> GetPublicByWorkspaceAsync(Guid workspaceId);
    Task<IReadOnlyList<Board>> GetByWorkspaceAndUserAsync(Guid workspaceId, Guid userId);
}