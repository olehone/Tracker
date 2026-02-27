using Domain.Entities;

namespace DataAccess.Abstractions;

public interface IBoardMetadataStorage
{
    Task<BoardMetadata?> GetAsync(Guid boardId, CancellationToken cancellationToken = default);

    Task<BoardMetadata> CreateAsync(BoardMetadata boardArchiveLog, CancellationToken cancellationToken = default);

    Task<BoardMetadata> UpdateAsync(BoardMetadata boardArchiveLog, CancellationToken cancellationToken = default);

    Task AppendLogAsync(Guid boardId, ArchiveLog log, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid boardId, CancellationToken cancellationToken = default);
}