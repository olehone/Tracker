using Tracker.Domain.Entities;

namespace Tracker.Application.Common.Repositories;

public interface IBoardRepository : IRepository<Board, Guid>
{
    Task<IReadOnlyList<Board>> GetByUserAsync(Guid userId);
    Task<Board?> GetWithWorkspaceAsync(Guid id);
    Task<Board?> GetWithWorkspaceByItemAsync(Guid itemId);
    Task<Board?> GetWithWorkspaceByListAsync(Guid listId);
    Task<Board?> GetWithWorkspaceByItemAttachmentAsync(Guid attachmentId);
    Task<Board?> GetByIdWithListsItemsUsersAsync(Guid id);
    Task<IReadOnlyList<Board>> GetPublicByWorkspaceAsync(Guid workspaceId);
    Task<IReadOnlyList<Board>> GetByWorkspaceAndUserAsync(Guid workspaceId, Guid userId);
}