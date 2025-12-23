using Tracker.Domain.Entities;

namespace Tracker.Application.Common.Repositories;

public interface IBoardRepository : IRepository<Board, Guid>
{
    Task<Board?> GetByIdWithListsAndItemsAsync(Guid id);
    Task<List<Board>> GetAllByWorkspaceId(Guid workspaceId);
}

