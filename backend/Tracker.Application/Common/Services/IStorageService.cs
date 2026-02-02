namespace Tracker.Application.Common.Services;

public interface IStorageService
{
    Task<Guid> UploadAsync(Stream stream, string contentType, CancellationToken cancellationToken = default);

    Task<FileResponse> DownloadAsync(Guid fieldId, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default);

}

public record FileResponse(Stream Stream, string ContentType);