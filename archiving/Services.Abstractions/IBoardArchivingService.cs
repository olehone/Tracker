namespace Services.Abstractions;

public interface IBoardArchivingService
{
    Task ArchiveBoardAsync(Guid boardId);
    Task UnarchiveBoardAsync(Guid boardId);
}