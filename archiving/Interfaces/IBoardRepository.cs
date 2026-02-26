using ArchivingFunction.Domain.Entities;
using ArchivingFunction.Domain.Enums;

namespace ArchivingFunction.Interfaces;

public interface IBoardRepository
{
    Task<Board?> LoadFullBoardAsync(Guid boardId);
    void UpdateBoardArchiveStatusAsync(Guid boardId, ArchiveStatus status);
    Task DeleteBoardContentAsync(Guid boardId);
    Task RestoreBoardContentAsync(Board snapshot);
    Task SaveChangesAsync();
}