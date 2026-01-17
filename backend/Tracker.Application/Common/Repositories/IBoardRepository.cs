using Tracker.Domain.Entities;

namespace Tracker.Application.Common.Repositories;

public interface IBoardRepository : IRepository<Board, Guid>
{
    Task<Board?> GetBoardWithWorkspaceAsync(Guid id);
    Task<Board?> GetBoardWithWorkspaceByItemAsync(Guid itemId);
    Task<Board?> GetBoardWithWorkspaceByListAsync(Guid listId);
    Task<Board?> GetByIdWithListsItemsUsersAsync(Guid id);
    Task<IReadOnlyList<Board>> GetPublicByWorkspaceAsync(Guid workspaceId);
    Task<IReadOnlyList<Board>> GetByWorkspaceAndUserAsync(Guid workspaceId, Guid userId);
}