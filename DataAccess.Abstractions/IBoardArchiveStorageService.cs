namespace DataAccess.Abstractions;

public interface IBoardArchiveStorageService
{
    Task<string> UploadAsync(Stream stream, string contentType, Guid resourceId, CancellationToken cancellationToken);
    Task<string?> GetUrlAsync(Guid resourceId, CancellationToken cancellationToken);
    Task DeleteAsync(Guid resourceId, CancellationToken cancellationToken = default);
}