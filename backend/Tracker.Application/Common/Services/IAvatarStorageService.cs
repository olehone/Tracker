namespace Tracker.Application.Common.Services;

// Public storage with public URL for file access
public interface IAvatarStorageService
{
    string GetUrl(Guid resourceId);
    Task<string> UploadAsync(Stream stream, string contentType, Guid resourceId, CancellationToken cancellationToken);
    Task DeleteAsync(Guid resourceId, CancellationToken cancellationToken = default);
}