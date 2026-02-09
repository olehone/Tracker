using Tracker.Domain.Results;

namespace Tracker.Application.Common.Services;

// Public storage with public URL for file access
public interface IAvatarStorageService
{
    Task<string> UploadAsync(Stream stream, string contentType, Guid resourceId, CancellationToken cancellationToken);
    Task<Result<string>> GetUrlAsync(Guid resourceId, CancellationToken cancellationToken);
    Task DeleteAsync(Guid resourceId, CancellationToken cancellationToken = default);
}