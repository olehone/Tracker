namespace DataAccess.Abstractions;

public interface IBoardRepository
{
    Task<Board?> LoadFullBoardAsync(Guid boardId);
    Task UpdateBoardArchiveStatusAsync(Guid boardId, ArchiveStatus status);
    Task DeleteBoardContentAsync(Guid boardId);
    void RestoreBoardContent(Board snapshot);
    Task SaveChangesAsync();
}